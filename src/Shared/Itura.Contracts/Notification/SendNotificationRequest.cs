namespace Itura.Contracts.Notification;

public sealed record SendNotificationRequest(
    Guid UserId,
    string Title,
    string Body,
    string Channel,
    string? RecipientEmail = null,
    string? RecipientName = null);
