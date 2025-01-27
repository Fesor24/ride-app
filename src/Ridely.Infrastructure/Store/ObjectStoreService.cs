using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Ridely.Application.Abstractions.Storage;

namespace Ridely.Infrastructure.Store;
internal sealed class ObjectStoreService : IObjectStoreService
{
    private readonly AmazonS3Client _s3;
    private readonly string _bucketName;

    public ObjectStoreService(IOptions<S3Options> s3Settings)
    {
        _s3 = new(s3Settings.Value.AccessKey,
            s3Settings.Value.SecretAccessKey, Amazon.RegionEndpoint.EUWest2);

        _bucketName = s3Settings.Value.BucketName;
    }

    public Task<(string Url, DateTime Expiry)> GeneratePreSignedUrl(string key)
    {
        var request = new GetPreSignedUrlRequest()
        {
            BucketName = _bucketName,
            Key = key
        };

        //if (expiry.HasValue)
        //    request.Expires = expiry.Value;

        request.Expires = DateTime.UtcNow.AddDays(5); // max seconds from s3 is 604800 (7 days)

        string url = _s3.GetPreSignedURL(request);

        return Task.FromResult((url, request.Expires));
    }

    public async Task<PutObjectResponse> UploadAsync(string key, IFormFile file)
    {
        var putObjectRequest = new PutObjectRequest()
        {
            Key = key,
            BucketName = _bucketName,
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream(),
            Metadata = {
                ["x-amz-meta-originalname"] = file.FileName,
                ["x-amz-meta-extension"] = Path.GetExtension(file.FileName)
            }
        };

        return await _s3.PutObjectAsync(putObjectRequest);
    }

    public async Task<PutObjectResponse> UploadAsync(string key, string base64Image)
    {
        // Remove the data URL prefix if present (e.g., "data:image/jpeg;base64,")
        // image base 64 string must start with the above format...so we know the type/format...
        var dataUrlPrefix = ";base64,";
        var base64Data = base64Image.Contains(dataUrlPrefix) ?
            base64Image.Substring(base64Image.IndexOf(dataUrlPrefix) + dataUrlPrefix.Length) : base64Image;

        // Decode Base64 string into byte array
        byte[] imageBytes = Convert.FromBase64String(base64Data);

        // Create a memory stream from the byte array
        using (var memoryStream = new MemoryStream(imageBytes))
        {
            memoryStream.Position = 0;

            var putObjectRequest = new PutObjectRequest()
            {
                Key = key,
                BucketName = _bucketName,
                ContentType = GetContentTypeFromBase64(base64Image), // Optional content type extraction
                InputStream = memoryStream,
                Metadata = {
                ["x-amz-meta-originalname"] = key,  // Since we don't have an original file, we can use the key as a name
                ["x-amz-meta-extension"] = GetFileExtensionFromBase64(base64Image) // You can extract the extension from the base64 if needed
            }
            };

            var response = await _s3.PutObjectAsync(putObjectRequest);

            return response;
        }
    }

    private string GetContentTypeFromBase64(string base64String)
    {
        if (base64String.StartsWith("data:image/jpeg")) return "image/jpeg";
        if (base64String.StartsWith("data:image/png")) return "image/png";
        if (base64String.StartsWith("data:image/jpg")) return "image/jpg";
        // Add more content types if needed
        throw new ApplicationException("Acceptable formats include: jpeg, png and jpg");
    }

    // Optionally, a method to get the file extension based on the Base64 string header
    private string GetFileExtensionFromBase64(string base64String)
    {
        if (base64String.StartsWith("data:image/jpeg")) return ".jpg";
        if (base64String.StartsWith("data:image/png")) return ".png";
        if (base64String.StartsWith("data:image/jpg")) return ".jpg";

        return string.Empty; // Default fallback
    }
}
