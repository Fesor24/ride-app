﻿using System.Net;
using System.Text.Json;
using Ridely.Application.Exceptions;
using Ridely.Domain.Abstractions;
using Ridely.Shared.Exceptions;
using Ridely.Shared.Helper;
using RidelyAPI.Shared;
using ValidationError = Ridely.Domain.Abstractions.ValidationError;

namespace RidelyAPI.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred. {Exception}", ex.Message);

            if (context.Response.HasStarted)
                await next(context);
            else
                context.Response.StatusCode = GetStatusCode(ex);

            ApiResponse response = GetErrorResponse(ex);

            string errorDetails = JsonSerializer.Serialize(response, SerializerOptions.Write);

            await context.Response.WriteAsync(errorDetails);
        }
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            ApiBadRequestException => (int)HttpStatusCode.BadRequest,
            ApiNotFoundException => (int)HttpStatusCode.NotFound,
            ApiUnauthorizedException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

    private static ApiResponse GetErrorResponse(Exception exception) =>
        exception switch
        {
            ValidationException ex => HandleValidationErrors(ex),
            ApiBadRequestException ex => new ApiResponse(new Error("bad.request", ex.Message)),
            ApiNotFoundException ex => new ApiResponse(new Error("not.found", ex.Message)),
            ApiUnauthorizedException ex => new ApiResponse(new Error("unauthorized", ex.Message)),
            _ => new ApiResponse(new Error("server.error", exception.Message))
        };

    private static ApiResponse HandleValidationErrors(Exception ex)
    {
        var defaultError = new Error("validation.failure", "One or more validation errors");

        if (string.IsNullOrWhiteSpace(ex.Message))
            return new(defaultError);

        var validationErrors = JsonSerializer
            .Deserialize<IEnumerable<Ridely.Application.Exceptions.ValidationError>>(ex.Message);

        if (validationErrors is null || !validationErrors.Any())
            return new(defaultError);

        var response = new ApiResponse(defaultError);

        response.Error.ValidationErrors = validationErrors
            .Select(error => new ValidationError(
                error.PropertyName, error.Error))
            .ToList();

        return response;
    }

}
