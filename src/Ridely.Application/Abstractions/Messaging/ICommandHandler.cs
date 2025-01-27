using MediatR;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Abstractions.Messaging;

internal interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result<bool>>
    where TCommand : ICommand;

internal interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
