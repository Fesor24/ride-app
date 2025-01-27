using Soloride.Domain.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Soloride.Infrastructure.Services;
internal class CacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
{
    private readonly IDatabase db = connectionMultiplexer.GetDatabase();

    public async Task<TObject?> GetAsync<TObject>(string key)
    {
        var value = await db.StringGetAsync(key);

        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<TObject>(value!);
    }

    public async Task<string?> GetAsync(string key) =>
        await db.StringGetAsync(key);

    public async Task<bool> RemoveAsync(string key) =>
        await db.KeyDeleteAsync(key);

    public async Task<bool> SetAsync(string key, string value, TimeSpan expiry) =>
        await db.StringSetAsync(key, value, expiry: expiry);
}
