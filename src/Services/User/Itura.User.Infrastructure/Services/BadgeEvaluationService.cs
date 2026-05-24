using Itura.User.Application.Common.Interfaces;
using Itura.User.Application.Features.Gamification;
using Itura.User.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Itura.User.Infrastructure.Services;

// Evaluates badge conditions after a trigger event.
// Condition format: "<metric> >= <threshold>"
// Supported metrics: xp, level, streak.<type>
internal sealed class BadgeEvaluationService(
    IBadgeRepository badgeRepository,
    IUserProfileRepository profileRepository,
    IStreakRepository streakRepository,
    ISender sender,
    ILogger<BadgeEvaluationService> logger) : IBadgeEvaluationService
{
    public async Task EvaluateAndAwardAsync(Guid userId, string trigger, CancellationToken ct = default)
    {
        var profile = await profileRepository.GetByIdAsync(userId, ct);
        if (profile is null) return;

        var definitions = await badgeRepository.GetAllDefinitionsAsync(ct);
        var candidates = definitions.Where(d =>
            string.IsNullOrEmpty(d.Trigger) ||
            d.Trigger.Equals(trigger, StringComparison.OrdinalIgnoreCase));

        foreach (var badge in candidates)
        {
            try
            {
                var met = await EvaluateConditionAsync(badge.Condition, profile.TotalXp, profile.WellnessLevel, userId, ct);
                if (!met) continue;

                var result = await sender.Send(new AwardBadgeCommand(userId, badge.Name), ct);
                if (result.IsSuccess && result.Value)
                    logger.LogInformation("Awarded badge '{Badge}' to user {UserId}", badge.Name, userId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error evaluating badge '{Badge}' for user {UserId}", badge.Name, userId);
            }
        }
    }

    private async Task<bool> EvaluateConditionAsync(
        string condition, int totalXp, int level, Guid userId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(condition)) return false;

        var parts = condition.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3) return false;

        var metric = parts[0].ToLowerInvariant();
        var op = parts[1];
        if (!int.TryParse(parts[2], out var threshold)) return false;

        int value;
        if (metric == "xp")
        {
            value = totalXp;
        }
        else if (metric == "level")
        {
            value = level;
        }
        else if (metric.StartsWith("streak."))
        {
            var streakType = metric["streak.".Length..];
            var streak = await streakRepository.GetAsync(userId, streakType, ct);
            value = streak?.CurrentStreak ?? 0;
        }
        else
        {
            return false;
        }

        return op switch
        {
            ">=" => value >= threshold,
            ">" => value > threshold,
            "==" => value == threshold,
            "<=" => value <= threshold,
            "<" => value < threshold,
            _ => false
        };
    }
}
