using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Admin.Driver.VerifyDriver;
public sealed record VerifyDriverCommand(string PhoneNo) : ICommand;
