using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Common.Calls.Command;

public record RouteCallCommand(string PhoneNo) : ICommand<string?>;