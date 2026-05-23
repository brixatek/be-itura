using Itura.Auth.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Itura.Auth.Infrastructure.Services;

public sealed class EmailOptions
{
    public const string Section = "Email";
    public string SendGridApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@itura.app";
    public string FromName { get; set; } = "Itura";
    public string AppBaseUrl { get; set; } = "https://app.itura.app";
}

internal sealed class EmailService(IOptions<EmailOptions> opts) : IEmailService
{
    private readonly EmailOptions _opts = opts.Value;

    public async Task SendEmailVerificationAsync(string toEmail, string fullName, string token, CancellationToken ct = default)
    {
        var link = $"{_opts.AppBaseUrl}/verify-email?token={Uri.EscapeDataString(token)}";
        var html = $"""
            <h2>Welcome to Itura, {fullName}!</h2>
            <p>Please verify your email to get started on your wellness journey.</p>
            <p><a href="{link}" style="background:#2D8A6D;color:#fff;padding:12px 24px;border-radius:8px;text-decoration:none;">Verify Email</a></p>
            <p>This link expires in 24 hours.</p>
            """;
        await SendAsync(toEmail, fullName, "Verify your Itura email", html, ct);
    }

    public async Task SendPasswordResetAsync(string toEmail, string fullName, string token, CancellationToken ct = default)
    {
        var link = $"{_opts.AppBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}";
        var html = $"""
            <h2>Reset your Itura password</h2>
            <p>We received a request to reset the password for your account.</p>
            <p><a href="{link}" style="background:#2D8A6D;color:#fff;padding:12px 24px;border-radius:8px;text-decoration:none;">Reset Password</a></p>
            <p>This link expires in 1 hour. If you didn't request this, please ignore this email.</p>
            """;
        await SendAsync(toEmail, fullName, "Reset your Itura password", html, ct);
    }

    public async Task SendPasswordChangedNotificationAsync(string toEmail, string fullName, CancellationToken ct = default)
    {
        var html = $"""
            <h2>Password changed</h2>
            <p>Your Itura password was changed. If this wasn't you, please contact support immediately at support@itura.app.</p>
            """;
        await SendAsync(toEmail, fullName, "Your Itura password was changed", html, ct);
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string htmlContent, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_opts.SendGridApiKey)) return; // Skip in dev without key

        var client = new SendGridClient(_opts.SendGridApiKey);
        var msg = MailHelper.CreateSingleEmail(
            new EmailAddress(_opts.FromEmail, _opts.FromName),
            new EmailAddress(toEmail, toName),
            subject,
            plainTextContent: null,
            htmlContent: htmlContent);

        await client.SendEmailAsync(msg, ct);
    }
}
