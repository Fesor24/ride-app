using MediatR;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Abstractions.Messaging;

internal interface ICommand : IRequest<Result<bool>>, IBaseCommand;

internal interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
