namespace Ridely.Application.Features.Admin.Driver.GetByPhoneNo;
public sealed class GetDriverByPhoneNoResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNo { get; set; }
    public string Email { get; set; }
    public string ProfileImageUrl { get; set; }
    public string LicenseImageUrl { get; set; }
    public bool IdentityValidated { get; set; }
    public int CompletedRides { get; set; }
    public string LicenseNo { get; set; }
    public CabResponse Cab { get; set; }

}

public sealed class CabResponse
{
    public string Color { get; set; }
    public string Year { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
}
