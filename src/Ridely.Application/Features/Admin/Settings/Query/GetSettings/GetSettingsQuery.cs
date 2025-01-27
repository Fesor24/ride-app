using MediatR;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Features.Admin.Settings.Query.GetSettings;
public record GetSettingsQuery : IRequest<Result<GetSettingsResponse>>;
