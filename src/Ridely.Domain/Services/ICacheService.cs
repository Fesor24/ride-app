namespace Ridely.Domain.Services;
public interface ICacheService
{
    Task<TObject?> GetAsync<TObject>(string key);
    Task<string?> GetAsync(string key);
    Task<bool> SetAsync(string key, string value, TimeSpan expiry);
    Task<bool> RemoveAsync(string key);
}
