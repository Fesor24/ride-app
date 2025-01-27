using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Admin.Driver.VerifyDriver;
public sealed record VerifyDriverCommand(string PhoneNo) : ICommand;
