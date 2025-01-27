using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.Storage;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Riders;
using Soloride.Shared.Exceptions;
using Soloride.Shared.Helper.Keys;

namespace Soloride.Application.Features.Common.UploadImage;
internal sealed class UploadImageCommandHandler :
    ICommandHandler<UploadImageCommand>
{
    private readonly IRiderRepository _riderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectStoreService _objectStoreService;

    public UploadImageCommandHandler(IRiderRepository riderRepository, IUnitOfWork unitOfWork,
        IObjectStoreService objectStoreService)
    {
        _riderRepository = riderRepository;
        _unitOfWork = unitOfWork;
        _objectStoreService = objectStoreService;
    }

    public async Task<Result<bool>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        string key = ComposeS3Key(request.Identifier, request.FileType);

        var response = await _objectStoreService.UploadAsync(key, request.File);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            return Error.BadRequest("upload.error", "An error occurred while uploading this file");

        if(request.FileType == FileType.RidersProfileImage)
        {
            DateTime expiry = DateTime.UtcNow.AddDays(7);  

            (string profileImageUrl, DateTime profileImageExpiry) = await 
                _objectStoreService.GeneratePreSignedUrl(key);

            var rider = await _riderRepository
                .GetAsync(request.Identifier);

            if (rider is null) return true;

            rider.UpdateProfileImageUrl(profileImageUrl, profileImageExpiry);

            _riderRepository.Update(rider);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    private static string ComposeS3Key(long identifier, FileType fileType) =>
        fileType switch
        {
            FileType.DriversLicense => UploadKeys.Driver.DriversLicense(identifier),
            FileType.DriversProfileImage => UploadKeys.Driver.ProfileImage(identifier),
            FileType.RidersProfileImage => UploadKeys.Rider.ProfileImage(identifier),
            _ => throw new ApiBadRequestException("Specify valid file type")
        };
}
