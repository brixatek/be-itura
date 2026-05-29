using Itura.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itura.Auth.Infrastructure.BackgroundJobs;

public sealed class HardDeleteExpiredAccountsJob(
    IServiceScopeFactory scopeFactory,
    ILogger<HardDeleteExpiredAccountsJob> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
    private const int RetentionDays = 90;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DeleteExpiredAccountsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during hard-delete of expired accounts");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task DeleteExpiredAccountsAsync(CancellationToken ct)
    {
        var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var deleted = await db.Accounts
            .IgnoreQueryFilters()
            .Where(a => a.DeletedAt != null && a.DeletedAt <= cutoff)
            .ExecuteDeleteAsync(ct);

        if (deleted > 0)
            logger.LogInformation("Hard-deleted {Count} accounts that exceeded {Days}-day retention window", deleted, RetentionDays);
    }
}
