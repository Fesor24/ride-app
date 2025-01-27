using MediatR;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Abstractions.Messaging;

internal interface IQueryHandler<TQuery> : IRequestHandler<TQuery, Result<bool>>
    where TQuery : IQuery;

internal interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
