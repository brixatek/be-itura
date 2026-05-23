using Itura.Mood.Application.DTOs;
using Itura.Mood.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Mood.Application.Features.Mood.GetSummary;

internal sealed class GetMoodSummaryQueryHandler(IMoodEntryRepository repository)
    : IRequestHandler<GetMoodSummaryQuery, Result<MoodSummaryDto>>
{
    public async Task<Result<MoodSummaryDto>> Handle(
        GetMoodSummaryQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var from = now.AddDays(-request.Days);

        var entries = (await repository.GetByUserIdForPeriodAsync(
            request.UserId, from, now, cancellationToken)).ToList();

        if (entries.Count == 0)
            return Result.Success(new MoodSummaryDto(0, 0, 0, "Stable", null, null, []));

        var avgScore = entries.Average(e => e.Score);
        var daysTracked = entries.Select(e => e.LoggedAt.Date).Distinct().Count();

        var thisWeekStart = now.AddDays(-7);
        var lastWeekStart = now.AddDays(-14);

        var thisWeekEntries = entries.Where(e => e.LoggedAt >= thisWeekStart).ToList();
        var lastWeekEntries = entries.Where(e => e.LoggedAt >= lastWeekStart && e.LoggedAt < thisWeekStart).ToList();

        double? thisWeekAvg = thisWeekEntries.Count > 0 ? thisWeekEntries.Average(e => e.Score) : null;
        double? lastWeekAvg = lastWeekEntries.Count > 0 ? lastWeekEntries.Average(e => e.Score) : null;

        var trend = "Stable";
        if (thisWeekAvg.HasValue && lastWeekAvg.HasValue)
        {
            if (thisWeekAvg > lastWeekAvg + 0.5) trend = "Improving";
            else if (thisWeekAvg < lastWeekAvg - 0.5) trend = "Declining";
        }

        var topEmotions = entries
            .SelectMany(e => e.Emotions)
            .GroupBy(e => e)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new EmotionCountDto(g.Key, g.Count()))
            .ToList();

        return Result.Success(new MoodSummaryDto(
            Math.Round(avgScore, 1),
            entries.Count,
            daysTracked,
            trend,
            thisWeekAvg.HasValue ? Math.Round(thisWeekAvg.Value, 1) : null,
            lastWeekAvg.HasValue ? Math.Round(lastWeekAvg.Value, 1) : null,
            topEmotions));
    }
}
