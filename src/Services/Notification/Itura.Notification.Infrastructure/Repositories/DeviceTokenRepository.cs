using Itura.Notification.Domain.Entities;
using Itura.Notification.Domain.Repositories;
using Itura.Notification.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Notification.Infrastructure.Repositories;

public sealed class DeviceTokenRepository(NotificationDbContext db) : IDeviceTokenRepository
{
    public async Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await db.DeviceTokens.FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task<List<DeviceToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await db.DeviceTokens.Where(t => t.UserId == userId && t.IsActive).ToListAsync(ct);

    public async Task AddAsync(DeviceToken token, CancellationToken ct = default)
        => await db.DeviceTokens.AddAsync(token, ct);

    public void Update(DeviceToken token) => db.DeviceTokens.Update(token);
}
