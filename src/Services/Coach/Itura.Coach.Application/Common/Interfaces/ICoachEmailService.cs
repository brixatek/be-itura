namespace Itura.Coach.Application.Common.Interfaces;

public interface ICoachEmailService
{
    Task SendApprovalEmailAsync(string coachEmail, string coachName, CancellationToken ct = default);
    Task SendRejectionEmailAsync(string coachEmail, string coachName, string reason, CancellationToken ct = default);
}
