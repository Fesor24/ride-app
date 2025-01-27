using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Drivers;
public class Cab : Entity
{
    private Cab()
    {

    }

    public Cab(string name, string manufacturer, string color, string model
        , string licensePlateNo, string year)
    {
        Name = name;
        Manufacturer = manufacturer;
        Color = color;
        Model = model;
        LicensePlateNo = licensePlateNo;
        Year = year;
        CabType = CabType.Economy; // todo: endpoint to update a car to premium
    }

    public string Name { get; private set; }
    public string Manufacturer { get; private set; }
    public string Color { get; private set; }
    public string Model { get; private set; }
    public string LicensePlateNo { get; private set; }
    public string Year { get; private set; }
    public CabType CabType { get; private set; }
}
