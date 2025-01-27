using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Admin.Driver.GetByPhoneNo;
public sealed record GetDriverByPhoneNoQuery(string PhoneNo) :
    IQuery<GetDriverByPhoneNoResponse>;
