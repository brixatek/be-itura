using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Application.DTOs;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

internal sealed class GetAvailableSlotsQueryHandler(
    IAvailabilityRepository repository,
    ISlotCache slotCache)
    : IRequestHandler<GetAvailableSlotsQuery, Result<List<TimeSlotDto>>>
{
    public async Task<Result<List<TimeSlotDto>>> Handle(
        GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var cached = await slotCache.GetAsync(request.CoachUserId, request.Date, request.ViewerTimezone ?? "UTC", cancellationToken);
        if (cached is not null)
            return Result.Success(cached);

        var dayOfWeek = request.Date.DayOfWeek;
        var schedule = await repository.GetByCoachUserIdAsync(request.CoachUserId, cancellationToken);
        var blocks = schedule.Where(a => a.DayOfWeek == dayOfWeek).ToList();

        if (blocks.Count == 0)
            return Result.Success(new List<TimeSlotDto>());

        // Fetch blocked times for the entire day (UTC range)
        var dayStartUtc = DateTime.SpecifyKind(request.Date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var dayEndUtc = dayStartUtc.AddDays(1);
        var blockedTimes = await repository.GetBlockedTimesAsync(
            request.CoachUserId, dayStartUtc, dayEndUtc, cancellationToken);

        var viewerTz = GetTimezone(request.ViewerTimezone);
        var slots = new List<TimeSlotDto>();

        foreach (var block in blocks)
        {
            var coachTz = GetTimezone(block.Timezone);

            // Build coach-local DateTime for this block on the requested date
            var blockStart = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day,
                block.StartTime.Hour, block.StartTime.Minute, 0, DateTimeKind.Unspecified);
            var blockEnd = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day,
                block.EndTime.Hour, block.EndTime.Minute, 0, DateTimeKind.Unspecified);

            // Convert to UTC
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(blockStart, coachTz);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(blockEnd, coachTz);

            var current = startUtc;
            while (current.AddMinutes(block.SlotDurationMinutes) <= endUtc)
            {
                var slotEnd = current.AddMinutes(block.SlotDurationMinutes);

                // Skip if overlaps any blocked time
                var isBlocked = blockedTimes.Any(bt => bt.StartUtc < slotEnd && bt.EndUtc > current);
                if (!isBlocked)
                {
                    var startLocal = TimeZoneInfo.ConvertTimeFromUtc(current, viewerTz);
                    var endLocal = TimeZoneInfo.ConvertTimeFromUtc(slotEnd, viewerTz);
                    slots.Add(new TimeSlotDto(
                        current, slotEnd,
                        startLocal.ToString("yyyy-MM-ddTHH:mm"),
                        endLocal.ToString("yyyy-MM-ddTHH:mm")));
                }

                current = slotEnd;
            }
        }

        var result = slots.OrderBy(s => s.StartUtc).ToList();
        await slotCache.SetAsync(request.CoachUserId, request.Date, request.ViewerTimezone ?? "UTC", result, cancellationToken);
        return Result.Success(result);
    }

    private static TimeZoneInfo GetTimezone(string? id)
    {
        if (string.IsNullOrEmpty(id)) return TimeZoneInfo.Utc;
        try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
        catch { return TimeZoneInfo.Utc; }
    }
}
