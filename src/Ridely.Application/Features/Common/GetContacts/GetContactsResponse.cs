namespace Soloride.Application.Features.Common.GetContacts;
public sealed class GetContactsResponse
{
    public SupportContact Support { get; set; }
    public List<string> Emergency { get; set; } = new();

}

public sealed class SupportContact
{
    public List<string> Emails { get; set; } = [];
    public List<string> Whatsapp { get; set; } = [];
}
