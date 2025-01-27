using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Models.Shared;

namespace Ridely.Application.Features.Drivers.GetOtp;

public sealed record GetOtpQuery(OtpReason OtpReason, long DriverId) : IQuery;
