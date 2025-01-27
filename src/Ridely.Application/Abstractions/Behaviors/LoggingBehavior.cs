﻿using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Abstractions.Behaviors;
internal sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest: IBaseRequest
    where TResponse: Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;

        _logger.LogInformation("Executing request {Request}", name);

        var result = await next();

        if (result.IsSuccessful)
            _logger.LogInformation("Request {Request} executed successfully", name);

        else
        {
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                _logger.LogError("Request {Request} processed with {@Error}", name, result.Error);
            }
        }

        return result;
    }
}
