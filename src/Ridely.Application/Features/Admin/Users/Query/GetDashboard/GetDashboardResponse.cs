namespace Soloride.Application.Features.Admin.Users.Query.GetDashboard;
public class GetDashboardResponse
{
    public DashboardUserResponse User { get; set; } = new();
    public DashboardRideResponse Ride { get; set; } = new();
}

public class DashboardUserResponse
{
    public int Drivers { get; set; }
    public int Riders { get; set; }
}

public class DashboardRideResponse
{
    public int CompletedRides { get; set; }
    public int ReassignedRides { get; set; }
    public int ReroutedRides { get; set; }
    public int RidesRequested { get; set; }
    public int RidesInTransit {  get; set; }
    public int CancelledRides { get; set; }
}
