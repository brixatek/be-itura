using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Itura.Notification.Infrastructure.Services;

public sealed class ApnsService(
    IConfiguration config,
    ILogger<ApnsService> logger)
{
    private readonly string _teamId = config["Apns:TeamId"] ?? string.Empty;
    private readonly string _keyId = config["Apns:KeyId"] ?? string.Empty;
    private readonly string _privateKeyPem = config["Apns:PrivateKeyPem"] ?? string.Empty;
    private readonly string _bundleId = config["Apns:BundleId"] ?? "app.itura";
    private readonly bool _useSandbox = bool.Parse(config["Apns:Sandbox"] ?? "false");

    private static readonly HttpClient Http = new(new SocketsHttpHandler
    {
        EnableMultipleHttp2Connections = true
    });

    public async Task<bool> SendAsync(string deviceToken, string title, string body,
        Dictionary<string, string>? data = null, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_teamId) || string.IsNullOrEmpty(_keyId) || string.IsNullOrEmpty(_privateKeyPem))
        {
            logger.LogInformation("[APNS-DEV] Push to iOS {Token}: {Title} — {Body}",
                deviceToken[..Math.Min(12, deviceToken.Length)], title, body);
            return true;
        }

        var host = _useSandbox
            ? "https://api.sandbox.push.apple.com"
            : "https://api.push.apple.com";

        var url = $"{host}/3/device/{deviceToken}";
        var jwt = GenerateJwt();

        var payload = new
        {
            aps = new
            {
                alert = new { title, body },
                sound = "default",
                badge = 1
            },
            data = data ?? new Dictionary<string, string>()
        };

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Version = new Version(2, 0);
            req.Headers.Authorization = new AuthenticationHeaderValue("bearer", jwt);
            req.Headers.TryAddWithoutValidation("apns-topic", _bundleId);
            req.Headers.TryAddWithoutValidation("apns-push-type", "alert");
            req.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8, "application/json");

            var resp = await Http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync(ct);
                logger.LogError("APNs push failed for token {Token}: {Status} {Error}",
                    deviceToken[..Math.Min(12, deviceToken.Length)], resp.StatusCode, err);
            }
            return resp.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "APNs push exception for token {Token}",
                deviceToken[..Math.Min(12, deviceToken.Length)]);
            return false;
        }
    }

    private string GenerateJwt()
    {
        var header = Base64UrlEncode(JsonSerializer.Serialize(new { alg = "ES256", kid = _keyId }));
        var payload = Base64UrlEncode(JsonSerializer.Serialize(new
        {
            iss = _teamId,
            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        }));

        var signingInput = $"{header}.{payload}";

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(_privateKeyPem);
        var signature = ecdsa.SignData(Encoding.UTF8.GetBytes(signingInput), HashAlgorithmName.SHA256);

        return $"{signingInput}.{Base64UrlEncode(signature)}";
    }

    private static string Base64UrlEncode(string input) =>
        Base64UrlEncode(Encoding.UTF8.GetBytes(input));

    private static string Base64UrlEncode(byte[] input) =>
        Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
