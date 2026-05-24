using Itura.Payment.Application.Features.CoachPayouts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itura.Payment.Infrastructure.BackgroundJobs;

public sealed class WeeklyPayoutJob(
    IServiceScopeFactory scopeFactory,
    ILogger<WeeklyPayoutJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            // Run every Monday at 09:00 UTC
            var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
            if (daysUntilMonday == 0 && now.Hour >= 9) daysUntilMonday = 7;
            var nextRun = now.Date.AddDays(daysUntilMonday).AddHours(9);
            var delay = nextRun - now;

            logger.LogInformation("WeeklyPayoutJob sleeping until {NextRun:u}", nextRun);
            await Task.Delay(delay, stoppingToken);

            await RunAsync(stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        logger.LogInformation("WeeklyPayoutJob running at {Time:u}", DateTime.UtcNow);
        try
        {
            using var scope = scopeFactory.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await sender.Send(new ProcessPayoutBatchCommand(), ct);

            if (result.IsSuccess)
                logger.LogInformation(
                    "WeeklyPayoutJob completed: {Processed} processed, {Failed} failed, {Total:C} total",
                    result.Value.Processed, result.Value.Failed, result.Value.TotalAmount);
            else
                logger.LogError("WeeklyPayoutJob failed: {Error}", result.Error);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "WeeklyPayoutJob threw an unhandled exception");
        }
    }
}
