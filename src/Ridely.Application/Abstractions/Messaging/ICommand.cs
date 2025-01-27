using MediatR;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Abstractions.Messaging;

internal interface ICommand : IRequest<Result<bool>>, IBaseCommand;

internal interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
