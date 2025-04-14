namespace Ridely.Domain.Models;
public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Sequence { get; set; }// todo: just for test...to be removed...
}

public class LocationAddressModel
{
    public string Address { get; set; }
}
