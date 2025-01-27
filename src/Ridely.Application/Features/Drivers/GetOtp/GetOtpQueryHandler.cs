using Microsoft.Extensions.Hosting;
using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.Notifications;
using Soloride.Application.Models.Shared;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Services;
using Soloride.Shared.Helper;
using Soloride.Shared.Helper.Keys;

namespace Soloride.Application.Features.Drivers.GetOtp;

internal sealed class GetOtpQueryHandler(IDriverRepository driverRepository, IHostEnvironment env,
    ICacheService cacheService, ISmsService smsService) : IQueryHandler<GetOtpQuery>
{
    public async Task<Result<bool>> Handle(GetOtpQuery request, CancellationToken cancellationToken)
    {
        var driver = await driverRepository
            .GetAsync(request.DriverId);

        if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

        string code = RandomGenerator.GenerateCode(6);
        if (env.IsDevelopment()) code = "222222";

        string key = string.Empty;

        if (request.OtpReason == OtpReason.AddDriverBankAccount)
            key = Cache.BankAccount.Key(driver.PhoneNo!);

        else if (request.OtpReason == OtpReason.DriverProcessWithdrawal)
            key = Cache.ProcessWithdrawal.Key(driver.PhoneNo!);

        else
            return Error.BadRequest("invalid.reason", "Select a valid reason");

        int codeExpiryInSeconds = 5 * 60;

        await cacheService.SetAsync(key, code, TimeSpan.FromSeconds(codeExpiryInSeconds));

        if (!env.IsDevelopment())
            await smsService.SendVerificationCodeAsync(driver.PhoneNo!, code, "5");

        return true;
    }
}
