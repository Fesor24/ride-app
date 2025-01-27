using AutoMapper;
using MediatR;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Common;

namespace Soloride.Application.Features.Admin.Settings.Query.GetSettings;
internal class GetSettingsQueryHandler(ISettingsRepository settingsRepository, IMapper mapper) : 
    IRequestHandler<GetSettingsQuery, Result<GetSettingsResponse>>
{
    public async Task<Result<GetSettingsResponse>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await settingsRepository.GetAllAsync();

        return mapper.Map<GetSettingsResponse>(settings.First());
    }
}
