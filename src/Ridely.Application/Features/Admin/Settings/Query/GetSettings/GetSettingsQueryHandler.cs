using AutoMapper;
using MediatR;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;

namespace Ridely.Application.Features.Admin.Settings.Query.GetSettings;
internal class GetSettingsQueryHandler(ISettingsRepository settingsRepository, IMapper mapper) : 
    IRequestHandler<GetSettingsQuery, Result<GetSettingsResponse>>
{
    public async Task<Result<GetSettingsResponse>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await settingsRepository.GetAllAsync();

        return mapper.Map<GetSettingsResponse>(settings.First());
    }
}
