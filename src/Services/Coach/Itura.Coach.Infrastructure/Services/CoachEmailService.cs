using Itura.Coach.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Itura.Coach.Infrastructure.Services;

internal sealed class CoachEmailService(ILogger<CoachEmailService> logger) : ICoachEmailService
{
    public Task SendApprovalEmailAsync(string coachEmail, string coachName, CancellationToken ct = default)
    {
        logger.LogInformation("Coach approved — email would be sent to {CoachEmail} ({CoachName})", coachEmail, coachName);
        return Task.CompletedTask;
    }

    public Task SendRejectionEmailAsync(string coachEmail, string coachName, string reason, CancellationToken ct = default)
    {
        logger.LogInformation("Coach rejected — email would be sent to {CoachEmail} ({CoachName}): {Reason}", coachEmail, coachName, reason);
        return Task.CompletedTask;
    }
}
