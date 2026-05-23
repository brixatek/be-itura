namespace Itura.Notification.Infrastructure.Services;

public sealed class EmailOptions
{
    public const string Section = "Email";
    public string SendGridApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@itura.app";
    public string FromName { get; set; } = "Itura";
    public string AppBaseUrl { get; set; } = "https://app.itura.app";
}
