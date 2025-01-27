using Microsoft.AspNetCore.Http;
using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Common.UploadImage;
public sealed record UploadImageCommand(IFormFile File, FileType FileType, long Identifier) :
    ICommand;

public enum FileType
{
    Unknown = 0,
    DriversLicense = 1,
    DriversProfileImage = 2,
    RidersProfileImage = 3,
}
