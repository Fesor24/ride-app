using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Features.Common.GetContacts;
internal sealed class GetContactsQueryHandler :
    IQueryHandler<GetContactSupportQuery, GetContactsResponse>
{
    public async Task<Result<GetContactsResponse>> Handle(GetContactSupportQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetContactsResponse()
        {
            Emergency = ["01-4931260", "01-4978899"],
            Support = new()
            {
                Emails = ["customercare@soloride.com"],
                Whatsapp = ["+23481909298283"]
            }
        };

        return await Task.FromResult(response);
    }
}
