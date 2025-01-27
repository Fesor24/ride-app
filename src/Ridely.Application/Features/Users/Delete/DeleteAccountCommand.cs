using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Users.Delete;
public sealed record DeleteAccountCommand(long? RiderId, long? DriverId) :
    ICommand;
