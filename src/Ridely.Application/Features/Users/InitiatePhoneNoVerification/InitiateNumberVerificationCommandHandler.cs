using Microsoft.Extensions.Hosting;
using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.Notifications;
using Soloride.Application.Extensions;
using Soloride.Application.Features.Accounts;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Services;
using Soloride.Shared.Helper;
using Soloride.Shared.Helper.Keys;

namespace Soloride.Application.Features.Users.InitiatePhoneNoVerification;
internal sealed class InitiateNumberVerificationCommandHandler :
    ICommandHandler<InitiateNumberVerificationCommand, InitiateNumberResponse>
{
    private readonly ICacheService _cacheService;
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IHostEnvironment _env;
    private readonly ISmsService _smsService;

    public InitiateNumberVerificationCommandHandler(ICacheService cacheService, IDriverRepository driverRepository,
        IRiderRepository riderRepository, IHostEnvironment env, ISmsService smsService)
    {
        _cacheService = cacheService;
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
        _env = env;
        _smsService = smsService;
    }

    public async Task<Result<InitiateNumberResponse>> Handle(InitiateNumberVerificationCommand request,
        CancellationToken cancellationToken)
    {
        string code = RandomGenerator.GenerateCode(6);

        string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";

        if (_env.IsDevelopment() || env == "Docker") code = "222222";

        int codeExpiryInSeconds = 5 * 60;

        string expiry = DateTime.UtcNow.AddSeconds(codeExpiryInSeconds).ToString();

        string key;

        string phoneNo = request.PhoneNo.ToPhoneNumber();

        if (request.AppInstance == ApplicationInstance.Rider)
        {
            var rider = await _riderRepository
                .GetByPhoneNoAsync(phoneNo);

            if (rider is null)
            {
                if (!_env.IsDevelopment())
                    await _smsService.SendVerificationCodeAsync(phoneNo, code, "5");

                key = Cache.UserAuth.Key(phoneNo, (int)request.AppInstance);

                await _cacheService.SetAsync(key, code, TimeSpan.FromSeconds(codeExpiryInSeconds));

                return new InitiateNumberResponse
                {
                    Code = code,
                    CodeExpiry = expiry,
                    NewUser = true,
                    CodeExpiryInSeconds = codeExpiryInSeconds
                };
            }
            else
            {
                if (rider.IsDeactivated)
                    return Error.BadRequest("account.deactivated", "User account deactivated. Contact support");

                if (rider.IsBarred)
                    return Error.BadRequest("account.barred", "User account barred. Contact support");

                if (!(_env.IsDevelopment() || env == "Docker"))
                    await _smsService.SendVerificationCodeAsync(rider.PhoneNo!, code, "5");

                key = Cache.UserAuth.Key(rider.PhoneNo!.ToPhoneNumber(), (int)request.AppInstance);

                await _cacheService.SetAsync(key, code, TimeSpan.FromSeconds(codeExpiryInSeconds));
            }
        }

        else if (request.AppInstance == ApplicationInstance.Driver)
        {
            var driver = await _driverRepository
                .GetByPhoneNoAsync(phoneNo);

            if (driver is null)
            {
                if (!_env.IsDevelopment())
                    await _smsService.SendVerificationCodeAsync(phoneNo, code, "5");

                key = Cache.UserAuth.Key(phoneNo, (int)request.AppInstance);

                await _cacheService.SetAsync(key, code, TimeSpan.FromSeconds(codeExpiryInSeconds));

                return new InitiateNumberResponse
                {
                    Code = code,
                    CodeExpiry = expiry,
                    CodeExpiryInSeconds = codeExpiryInSeconds,
                    NewUser = true
                };
            }
            else
            {
                if (driver.IsDeactivated)
                    return Error.BadRequest("account.deactivated", "User account deactivated");

                if (driver.IsBarred)
                    return Error.BadRequest("account.barred", "User account barred");

                if (!(_env.IsDevelopment() || env == "Docker"))
                    await _smsService.SendVerificationCodeAsync(driver.PhoneNo!, code, "5");

                key = Cache.UserAuth.Key(phoneNo.ToPhoneNumber(), (int)request.AppInstance);

                await _cacheService.SetAsync(key, code, TimeSpan.FromSeconds(codeExpiryInSeconds));
            }
        }

        else
            return Error.BadRequest("invalid.appinstance", "Specify a valid app instance");

        return new InitiateNumberResponse
        {
            Code = code,
            CodeExpiry = expiry,
            CodeExpiryInSeconds = codeExpiryInSeconds,
            NewUser = false
        };
    }
}
