using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Admin.Users.Query.GetDashboard;
public sealed record GetDashboardQuery(long RoleId) : IQuery<GetDashboardResponse>;
