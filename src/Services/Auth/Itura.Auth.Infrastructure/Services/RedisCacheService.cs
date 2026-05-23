using Itura.Auth.Application.Common.Interfaces;
using StackExchange.Redis;

namespace Itura.Auth.Infrastructure.Services;

internal sealed class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private IDatabase Db => redis.GetDatabase();
    private const string JtiPrefix = "auth:jti:blacklist:";

    public Task SetAsync(string key, string value, TimeSpan expiry, CancellationToken ct = default) =>
        Db.StringSetAsync(key, value, expiry);

    public async Task<string?> GetAsync(string key, CancellationToken ct = default) =>
        (await Db.StringGetAsync(key)).ToString();

    public Task<bool> ExistsAsync(string key, CancellationToken ct = default) =>
        Db.KeyExistsAsync(key);

    public Task DeleteAsync(string key, CancellationToken ct = default) =>
        Db.KeyDeleteAsync(key);

    public Task BlacklistJtiAsync(string jti, TimeSpan expiry, CancellationToken ct = default) =>
        Db.StringSetAsync($"{JtiPrefix}{jti}", "1", expiry);

    public Task<bool> IsJtiBlacklistedAsync(string jti, CancellationToken ct = default) =>
        Db.KeyExistsAsync($"{JtiPrefix}{jti}");
}
