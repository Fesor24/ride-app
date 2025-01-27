using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Models.Shared;

namespace Soloride.Application.Features.Drivers.GetOtp;

public sealed record GetOtpQuery(OtpReason OtpReason, long DriverId) : IQuery;
