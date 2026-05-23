using Itura.User.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itura.User.Infrastructure.BackgroundJobs;

public sealed class CheckStreakAtRiskJob(
    IServiceScopeFactory scopeFactory,
    ILogger<CheckStreakAtRiskJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = now.Date.AddDays(1).AddHours(19); // 7 PM UTC daily
            if (now.Hour >= 19) nextRun = nextRun.AddDays(1);
            var delay = nextRun - now;

            logger.LogInformation("CheckStreakAtRiskJob sleeping until {NextRun:u}", nextRun);
            await Task.Delay(delay, stoppingToken);

            await RunAsync(stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        logger.LogInformation("CheckStreakAtRiskJob running at {Time:u}", DateTime.UtcNow);
        try
        {
            using var scope = scopeFactory.CreateScope();
            var streakRepo = scope.ServiceProvider.GetRequiredService<IStreakRepository>();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var atRisk = await streakRepo.GetAtRiskAsync(today, ct);

            logger.LogInformation("Found {Count} users with streaks at risk", atRisk.Count);

            // Publish StreakRiskEvent per user — notification-service sends push at 7pm
            // Currently logging; integrate MassTransit publish when notification-service is ready
            foreach (var streak in atRisk)
            {
                logger.LogInformation(
                    "Streak at risk: UserId={UserId} Type={Type} Current={Current}",
                    streak.UserProfileId, streak.StreakType, streak.CurrentStreak);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "CheckStreakAtRiskJob failed");
        }
    }
}
