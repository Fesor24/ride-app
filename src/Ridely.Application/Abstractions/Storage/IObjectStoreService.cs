using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace Ridely.Application.Abstractions.Storage;
public interface IObjectStoreService
{
    Task<PutObjectResponse> UploadAsync(string key, IFormFile file);
    Task<PutObjectResponse> UploadAsync(string key, string base64Image);

    Task<(string Url, DateTime Expiry)> GeneratePreSignedUrl(string key);
}
