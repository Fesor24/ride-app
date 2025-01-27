using MediatR;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Abstractions.Messaging;

internal interface IQuery : IRequest<Result<bool>>;

internal interface IQuery<TResponse> : IRequest<Result<TResponse>>;
