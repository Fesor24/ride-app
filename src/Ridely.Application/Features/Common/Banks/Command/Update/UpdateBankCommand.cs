using MediatR;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Features.Common.Banks.Command.Update;
public record UpdateBankCommand(
    int Id,
    string Name) : IRequest<Result<bool>>;
