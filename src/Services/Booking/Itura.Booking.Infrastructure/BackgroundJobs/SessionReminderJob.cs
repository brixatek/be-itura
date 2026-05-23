using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itura.Booking.Infrastructure.BackgroundJobs;

public sealed class SessionReminderJob(
    IServiceScopeFactory scopeFactory,
    ILogger<SessionReminderJob> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Interval, stoppingToken);
            await RunAsync(stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var uow = scope.ServiceProvider.GetRequiredService<IBookingUnitOfWork>();

            var now = DateTime.UtcNow;

            // 24-hour reminders: sessions in next 24h–25h window
            var window24From = now.AddHours(24);
            var window24To = now.AddHours(25);
            var sessions24h = await repo.GetPendingRemindersAsync(window24From, window24To, is24h: true, ct);
            foreach (var session in sessions24h)
            {
                session.MarkReminder24hSent();
                repo.Update(session);
                logger.LogInformation("24h reminder — BookingId={BookingId} Client={ClientUserId} Coach={CoachUserId} At={ScheduledAt:u}",
                    session.Id, session.ClientUserId, session.CoachUserId, session.ScheduledAt);
            }

            // 1-hour reminders: sessions in next 1h–1h15m window
            var window1From = now.AddHours(1);
            var window1To = now.AddMinutes(75);
            var sessions1h = await repo.GetPendingRemindersAsync(window1From, window1To, is24h: false, ct);
            foreach (var session in sessions1h)
            {
                session.MarkReminder1hSent();
                repo.Update(session);
                logger.LogInformation("1h reminder — BookingId={BookingId} Client={ClientUserId} Coach={CoachUserId} At={ScheduledAt:u}",
                    session.Id, session.ClientUserId, session.CoachUserId, session.ScheduledAt);
            }

            if (sessions24h.Count + sessions1h.Count > 0)
                await uow.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "SessionReminderJob failed");
        }
    }
}
