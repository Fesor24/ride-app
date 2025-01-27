using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Common.GetContacts;
public sealed record GetContactSupportQuery() :
    IQuery<GetContactsResponse>;
