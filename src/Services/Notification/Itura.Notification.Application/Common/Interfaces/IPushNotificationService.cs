namespace Itura.Notification.Application.Common.Interfaces;

public interface IPushNotificationService
{
    Task<bool> SendAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
    Task<int> SendToUserAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
}
