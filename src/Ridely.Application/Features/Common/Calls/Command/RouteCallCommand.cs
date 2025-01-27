using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Common.Calls.Command;

public record RouteCallCommand(string PhoneNo) : ICommand<string?>;