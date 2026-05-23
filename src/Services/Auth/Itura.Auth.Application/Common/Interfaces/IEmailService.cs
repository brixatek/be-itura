namespace Itura.Auth.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string toEmail, string fullName, string token, CancellationToken ct = default);
    Task SendPasswordResetAsync(string toEmail, string fullName, string token, CancellationToken ct = default);
    Task SendPasswordChangedNotificationAsync(string toEmail, string fullName, CancellationToken ct = default);
}
