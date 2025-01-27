using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Rides.DriverSourceArrival;
public sealed record DriverSourceArrivalCommand(long RideId) : ICommand;
