using System.Data;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Soloride.Application.Abstractions.Data;
using Soloride.Domain.Abstractions;

namespace Soloride.Infrastructure.Outbox;
public sealed class ProcessOutboxMessagesJob
{
    private readonly IPublisher _publisher;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    private readonly OutboxOptions _outboxOptions;

    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
    };

    public ProcessOutboxMessagesJob(
        IOptions<OutboxOptions> outboxOptions, IPublisher publisher, 
        ISqlConnectionFactory sqlConnectionFactory, ILogger<ProcessOutboxMessagesJob> logger)
    {
        _publisher = publisher;
        _sqlConnectionFactory = sqlConnectionFactory;
        _logger = logger;
        _outboxOptions = outboxOptions.Value;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Processing outbox messages start");

        using var connection = _sqlConnectionFactory.CreateConnection();
        var transaction = connection.BeginTransaction();

        var outboxMessages = await GetOutboxMessages(connection, transaction);

        if (outboxMessages.Count == 0)
        {
            _logger.LogInformation("No outbox message to process");
            return;
        }

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content, JsonSerializerSettings)!;

                await _publisher.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception while processing outbox messages {MessageId}", outboxMessage.Id);

                exception = ex;
            }

            await UpdateOutboxMessagesAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        _logger.LogInformation("Completed processing outbox messages");

    }

    private async Task UpdateOutboxMessagesAsync(IDbConnection connection, IDbTransaction transaction, 
        OutboxMessageResponse outboxMessage, Exception? exception)
    {
        const string sql = @"
            UPDATE com.""OutboxMessages""
            SET ""ProcessedOnUtc"" = @ProcessedOnUtc,
                ""Error"" = @Error
            WHERE ""Id"" = @Id
";

        await connection.ExecuteAsync(sql, new
        {
            outboxMessage.Id,
            Error = exception?.ToString(),
            ProcessedOnUtc = DateTime.UtcNow
        }, transaction);
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessages(IDbConnection connection, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT "Id", "Content"
            FROM com."OutboxMessages"
            WHERE "ProcessedOnUtc" IS NULL
            ORDER BY "OccurredAtUtc"
            LIMIT {_outboxOptions.IntervalInSeconds}
            FOR UPDATE
            """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);

        return outboxMessages.ToList();
    }

    internal sealed record OutboxMessageResponse(long Id, string Content);
}
