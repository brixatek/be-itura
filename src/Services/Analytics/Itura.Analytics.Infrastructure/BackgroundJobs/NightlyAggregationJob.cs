using Itura.Analytics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itura.Analytics.Infrastructure.BackgroundJobs;

// Runs nightly at 01:00 UTC to aggregate analytics_events into daily summary counts.
// Stores results in analytics_daily_summary (created if missing via raw SQL).
public sealed class NightlyAggregationJob(
    IServiceScopeFactory scopeFactory,
    ILogger<NightlyAggregationJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = now.Date.AddDays(1).AddHours(1); // 01:00 UTC next day
            if (now.Hour < 1) nextRun = now.Date.AddHours(1); // same day if before 01:00

            var delay = nextRun - now;
            logger.LogInformation("NightlyAggregationJob: next run at {NextRun} (in {Delay})", nextRun, delay);

            try { await Task.Delay(delay, stoppingToken); }
            catch (OperationCanceledException) { break; }

            await RunAggregationAsync(stoppingToken);
        }
    }

    private async Task RunAggregationAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();

        var targetDate = DateTime.UtcNow.Date.AddDays(-1); // aggregate yesterday

        try
        {
            // Ensure summary table exists
            await db.Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS itura_analytics.analytics_daily_summary (
                    summary_date DATE NOT NULL,
                    event_type VARCHAR(100) NOT NULL,
                    event_count INT NOT NULL,
                    PRIMARY KEY (summary_date, event_type)
                );
                """, ct);

            // Upsert aggregated counts for yesterday
            await db.Database.ExecuteSqlRawAsync("""
                INSERT INTO itura_analytics.analytics_daily_summary (summary_date, event_type, event_count)
                SELECT DATE("OccurredAt") AS summary_date, "EventType" AS event_type, COUNT(*) AS event_count
                FROM itura_analytics.analytics_events
                WHERE DATE("OccurredAt") = {0}
                GROUP BY DATE("OccurredAt"), "EventType"
                ON CONFLICT (summary_date, event_type)
                DO UPDATE SET event_count = EXCLUDED.event_count;
                """, [targetDate], ct);

            logger.LogInformation("NightlyAggregationJob: aggregated events for {Date}", targetDate.ToString("yyyy-MM-dd"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "NightlyAggregationJob: aggregation failed for {Date}", targetDate.ToString("yyyy-MM-dd"));
        }
    }
}
