using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Soloride.Domain.Abstractions;
using Soloride.Infrastructure.Outbox;

namespace Soloride.Infrastructure;
internal sealed class ApplicationDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    internal const string User = "usr";
    internal const string Rides = "rds";
    internal const string Common = "com";
    internal const string Transaction = "trx";
    internal const string Driver = "drv";
    internal const string Rider = "rdr";

    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(s => s.GetProperties())
            .Where(x => x.ClrType == typeof(decimal) || x.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddDomainEventsAsOutboxMessages();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        //List<OutboxMessage> outboxMessages = [];

        //foreach (var entry in ChangeTracker.Entries<Entity>())
        //{
        //    Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");

        //    var events = entry.Entity.GetDomainEvents();

        //    outboxMessages.AddRange(events.Select(ev => new OutboxMessage
        //    (ev.GetType().Name,
        //    JsonConvert.SerializeObject(ev, JsonSerializerSettings))
        //    ));
        //}

        var outboxMessages = ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();

        AddRange(outboxMessages);
    }
}
