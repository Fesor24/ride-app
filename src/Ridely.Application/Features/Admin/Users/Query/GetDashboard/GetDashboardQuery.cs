using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Admin.Users.Query.GetDashboard;
public sealed record GetDashboardQuery(long RoleId) : IQuery<GetDashboardResponse>;
