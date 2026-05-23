namespace Itura.Notification.Application.DTOs;

public sealed record NotificationDto(
    Guid Id,
    Guid UserId,
    string Title,
    string Body,
    string Type,
    string Channel,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt);
