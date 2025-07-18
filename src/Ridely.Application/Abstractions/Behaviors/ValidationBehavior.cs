﻿using FluentValidation;
using MediatR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Exceptions;

namespace Ridely.Application.Abstractions.Behaviors;
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationErrors = validators
             .Select(validator => validator.Validate(context))
             .Where(validationResult => validationResult.Errors.Any())
             .SelectMany(validationResult => validationResult.Errors)
             .Select(validationFailure => new ValidationError(
                 validationFailure.PropertyName, validationFailure.ErrorMessage))
             .ToList();

        if (validationErrors.Any())
            throw new Exceptions.ValidationException(validationErrors);

        return await next();
    }
}
