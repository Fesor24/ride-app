using MediatR;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Features.Common.Banks.Command.Update;
public record UpdateBankCommand(
    int Id,
    string Name) : IRequest<Result<bool>>;
