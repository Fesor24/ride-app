using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Common.GetContacts;
public sealed record GetContactSupportQuery() :
    IQuery<GetContactsResponse>;
