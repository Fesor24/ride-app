using MediatR;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Abstractions.Messaging;

internal interface IQuery : IRequest<Result<bool>>;

internal interface IQuery<TResponse> : IRequest<Result<TResponse>>;
