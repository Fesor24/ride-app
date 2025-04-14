using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Storage;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Shared.Exceptions;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Common.UploadImage;
internal sealed class UploadImageCommandHandler :
    ICommandHandler<UploadImageCommand>
{
    private readonly IRiderRepository _riderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectStoreService _objectStoreService;

    public UploadImageCommandHandler(IRiderRepository riderRepository, IDriverRepository driverRepository,
        IUnitOfWork unitOfWork, IObjectStoreService objectStoreService)
    {
        _riderRepository = riderRepository;
        _driverRepository = driverRepository;
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

        else if(request.FileType == FileType.DriversProfileImage)
        {
            (string profileImageUrl, DateTime profileImageExpiry) = await
                _objectStoreService.GeneratePreSignedUrl(key);

            var driver = await _driverRepository
                .GetAsync(request.Identifier);

            if (driver is null) return true;

            driver.UploadImages(profileImageUrl, string.Empty, profileImageExpiry);

            _driverRepository.Update(driver);

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
