using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Itura.Notification.Infrastructure.Services;

public sealed class FirebasePushService(
    IDeviceTokenRepository tokenRepository,
    ApnsService apnsService,
    IConfiguration config,
    ILogger<FirebasePushService> logger) : IPushNotificationService
{
    private readonly string _serverKey = config["Firebase:ServerKey"] ?? string.Empty;
    private static readonly HttpClient Http = new();

    public async Task<bool> SendAsync(string deviceToken, string title, string body,
        Dictionary<string, string>? data = null, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_serverKey))
        {
            logger.LogInformation("[FCM-DEV] Push to {Token}: {Title} — {Body}", deviceToken[..Math.Min(12, deviceToken.Length)], title, body);
            return true;
        }

        var payload = new
        {
            to = deviceToken,
            notification = new { title, body },
            data = data ?? new Dictionary<string, string>()
        };

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
            req.Headers.TryAddWithoutValidation("Authorization", $"key={_serverKey}");
            req.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8, "application/json");

            var resp = await Http.SendAsync(req, ct);
            return resp.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "FCM push failed for token {Token}", deviceToken[..Math.Min(12, deviceToken.Length)]);
            return false;
        }
    }

    public async Task<int> SendToUserAsync(Guid userId, string title, string body,
        Dictionary<string, string>? data = null, CancellationToken ct = default)
    {
        var tokens = await tokenRepository.GetActiveByUserIdAsync(userId, ct);
        int sent = 0;
        foreach (var dt in tokens)
        {
            var ok = dt.Platform == "ios"
                ? await apnsService.SendAsync(dt.Token, title, body, data, ct)
                : await SendAsync(dt.Token, title, body, data, ct);
            if (ok) sent++;
        }
        return sent;
    }
}
