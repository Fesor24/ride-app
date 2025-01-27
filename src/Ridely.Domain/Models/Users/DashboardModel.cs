namespace Soloride.Domain.Models.Users;
public class DashboardModel
{
    public int Riders { get; set; }
    public int Drivers { get; set; }
    public int CompletedRides { get; set; }
    public int ReroutedRides { get; set; }
    public int RidesRequested { get; set; }
}
