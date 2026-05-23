namespace Itura.Auth.Application.Common.Interfaces;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan expiry, CancellationToken ct = default);
    Task<string?> GetAsync(string key, CancellationToken ct = default);
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
    Task DeleteAsync(string key, CancellationToken ct = default);

    // JWT revocation — blacklist access tokens by jti
    Task BlacklistJtiAsync(string jti, TimeSpan expiry, CancellationToken ct = default);
    Task<bool> IsJtiBlacklistedAsync(string jti, CancellationToken ct = default);
}
