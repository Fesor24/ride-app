using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Common.Banks.Query.GetAll;
public sealed record GetBanksQuery() : IQuery<List<GetBankResponse>>;
