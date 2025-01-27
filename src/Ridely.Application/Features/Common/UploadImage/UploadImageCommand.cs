using Microsoft.AspNetCore.Http;
using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Common.UploadImage;
public sealed record UploadImageCommand(IFormFile File, FileType FileType, long Identifier) :
    ICommand;

public enum FileType
{
    Unknown = 0,
    DriversLicense = 1,
    DriversProfileImage = 2,
    RidersProfileImage = 3,
}
