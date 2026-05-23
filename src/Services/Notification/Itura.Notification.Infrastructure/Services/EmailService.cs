using Itura.Notification.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Itura.Notification.Infrastructure.Services;

internal sealed class EmailService(IOptions<EmailOptions> opts) : IEmailService
{
    private readonly EmailOptions _opts = opts.Value;

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_opts.SendGridApiKey)) return;

        var client = new SendGridClient(_opts.SendGridApiKey);
        var msg = MailHelper.CreateSingleEmail(
            new EmailAddress(_opts.FromEmail, _opts.FromName),
            new EmailAddress(toEmail, toName),
            subject,
            plainTextContent: null,
            htmlContent: htmlBody);

        await client.SendEmailAsync(msg, ct);
    }
}
