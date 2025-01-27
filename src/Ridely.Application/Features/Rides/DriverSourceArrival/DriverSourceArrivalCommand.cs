using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.DriverSourceArrival;
public sealed record DriverSourceArrivalCommand(long RideId) : ICommand;
