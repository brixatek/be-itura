using Itura.Coach.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Itura.Coach.Infrastructure.Services;

internal sealed class CoachEmailService(
    IConfiguration configuration,
    ILogger<CoachEmailService> logger) : ICoachEmailService
{
    private string SendGridApiKey => configuration["Email:SendGridApiKey"] ?? string.Empty;
    private string FromEmail => configuration["Email:FromEmail"] ?? "noreply@itura.app";
    private string FromName => configuration["Email:FromName"] ?? "Itura";

    public async Task SendApprovalEmailAsync(string coachEmail, string coachName, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(coachEmail))
        {
            logger.LogWarning("Coach approval email skipped — no email address for coach {CoachName}", coachName);
            return;
        }

        var html = $"""
            <h2>Congratulations, {coachName}! Your Itura coach profile has been approved.</h2>
            <p>You can now accept bookings and start helping clients on their wellness journey.</p>
            <p>Log in to your dashboard to complete your availability setup and start accepting sessions.</p>
            <p>Welcome to the Itura coaching team!</p>
            """;

        await SendAsync(coachEmail, coachName, "Your Itura coach profile has been approved!", html, ct);
    }

    public async Task SendRejectionEmailAsync(string coachEmail, string coachName, string reason, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(coachEmail))
        {
            logger.LogWarning("Coach rejection email skipped — no email address for coach {CoachName}", coachName);
            return;
        }

        var html = $"""
            <h2>Update on your Itura coach application, {coachName}</h2>
            <p>Thank you for applying to become an Itura coach. After reviewing your profile, we are unable to approve it at this time.</p>
            <p><strong>Reason:</strong> {reason}</p>
            <p>You are welcome to update your profile and reapply. If you have questions, please contact support@itura.app.</p>
            """;

        await SendAsync(coachEmail, coachName, "Update on your Itura coach application", html, ct);
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string htmlContent, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(SendGridApiKey))
        {
            logger.LogInformation("SendGrid API key not configured — email to {Email} skipped", toEmail);
            return;
        }

        var client = new SendGridClient(SendGridApiKey);
        var msg = MailHelper.CreateSingleEmail(
            new EmailAddress(FromEmail, FromName),
            new EmailAddress(toEmail, toName),
            subject,
            plainTextContent: null,
            htmlContent: htmlContent);

        var response = await client.SendEmailAsync(msg, ct);
        if (!response.IsSuccessStatusCode)
            logger.LogError("Failed to send coach email to {Email}: status {Status}", toEmail, response.StatusCode);
    }
}
