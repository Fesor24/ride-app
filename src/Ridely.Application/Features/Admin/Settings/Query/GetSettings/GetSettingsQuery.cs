using MediatR;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Features.Admin.Settings.Query.GetSettings;
public record GetSettingsQuery : IRequest<Result<GetSettingsResponse>>;
