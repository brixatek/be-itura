using Itura.User.Application.DTOs;

namespace Itura.User.Application.Common.Interfaces;

public interface IPreferencesCache
{
    Task<UserPreferencesDto?> GetAsync(Guid userId, CancellationToken ct = default);
    Task SetAsync(Guid userId, UserPreferencesDto preferences, CancellationToken ct = default);
    Task RemoveAsync(Guid userId, CancellationToken ct = default);
}
