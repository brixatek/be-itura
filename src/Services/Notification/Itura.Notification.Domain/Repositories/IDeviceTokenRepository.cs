using Itura.Notification.Domain.Entities;

namespace Itura.Notification.Domain.Repositories;

public interface IDeviceTokenRepository
{
    Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<List<DeviceToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(DeviceToken token, CancellationToken ct = default);
    void Update(DeviceToken token);
}
