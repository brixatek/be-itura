using Itura.User.Application.Common.Interfaces;
using Itura.User.Application.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Itura.User.Infrastructure.Services;

internal sealed class PreferencesCache(IDistributedCache cache) : IPreferencesCache
{
    private static string Key(Guid userId) => $"prefs:{userId:N}";

    private static readonly DistributedCacheEntryOptions Ttl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
    };

    public async Task<UserPreferencesDto?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var json = await cache.GetStringAsync(Key(userId), ct);
        return json is null ? null : JsonSerializer.Deserialize<UserPreferencesDto>(json);
    }

    public async Task SetAsync(Guid userId, UserPreferencesDto preferences, CancellationToken ct = default) =>
        await cache.SetStringAsync(Key(userId), JsonSerializer.Serialize(preferences), Ttl, ct);

    public Task RemoveAsync(Guid userId, CancellationToken ct = default) =>
        cache.RemoveAsync(Key(userId), ct);
}
