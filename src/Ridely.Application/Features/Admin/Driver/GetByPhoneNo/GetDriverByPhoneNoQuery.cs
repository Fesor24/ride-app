using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Admin.Driver.GetByPhoneNo;
public sealed record GetDriverByPhoneNoQuery(string PhoneNo) :
    IQuery<GetDriverByPhoneNoResponse>;
