using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Common.Banks.Query.GetAll;
public sealed record GetBanksQuery() : IQuery<List<GetBankResponse>>;
