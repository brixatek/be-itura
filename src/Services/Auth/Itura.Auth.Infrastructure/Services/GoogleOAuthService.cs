using Itura.Auth.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Itura.Auth.Infrastructure.Services;

internal sealed class GoogleOAuthService(
    IHttpClientFactory httpClientFactory,
    IOptions<GoogleOAuthOptions> options,
    ILogger<GoogleOAuthService> logger)
    : IGoogleOAuthService
{
    private readonly GoogleOAuthOptions _opts = options.Value;

    public async Task<GoogleUserInfo?> ExchangeCodeAsync(
        string code, string redirectUri, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_opts.ClientId) || string.IsNullOrEmpty(_opts.ClientSecret))
        {
            logger.LogWarning("[DEV] Google OAuth not configured — returning mock user");
            return new GoogleUserInfo("google_dev_mock", "dev@example.com", "Dev User", null, true);
        }

        try
        {
            // Step 1: Exchange code for tokens
            var client = httpClientFactory.CreateClient("Google");
            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = _opts.ClientId,
                ["client_secret"] = _opts.ClientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            });

            var tokenResponse = await client.PostAsync(_opts.TokenEndpoint, tokenRequest, ct);
            tokenResponse.EnsureSuccessStatusCode();

            var tokenData = await tokenResponse.Content.ReadFromJsonAsync<GoogleTokenResponse>(cancellationToken: ct);
            if (tokenData?.IdToken is null)
            {
                logger.LogError("Google token exchange did not return an id_token");
                return null;
            }

            // Step 2: Verify ID token using JWKS
            var userInfo = await VerifyIdTokenAsync(tokenData.IdToken, ct);
            return userInfo;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Google OAuth exchange failed");
            return null;
        }
    }

    private async Task<GoogleUserInfo?> VerifyIdTokenAsync(string idToken, CancellationToken ct)
    {
        // Fetch Google's public JWKS
        var client = httpClientFactory.CreateClient("Google");
        var jwksJson = await client.GetStringAsync(_opts.JwksUri, ct);
        var jwks = new JsonWebKeySet(jwksJson);

        var handler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = ["https://accounts.google.com", "accounts.google.com"],
            ValidateAudience = true,
            ValidAudience = _opts.ClientId,
            ValidateLifetime = true,
            IssuerSigningKeys = jwks.GetSigningKeys(),
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        var principal = handler.ValidateToken(idToken, validationParams, out _);

        var sub = principal.FindFirst("sub")?.Value;
        var email = principal.FindFirst("email")?.Value;
        var emailVerified = principal.FindFirst("email_verified")?.Value == "true";
        var name = principal.FindFirst("name")?.Value;
        var picture = principal.FindFirst("picture")?.Value;

        if (sub is null || email is null) return null;

        return new GoogleUserInfo(sub, email, name, picture, emailVerified);
    }

    private sealed record GoogleTokenResponse(
        [property: JsonPropertyName("access_token")] string? AccessToken,
        [property: JsonPropertyName("id_token")] string? IdToken,
        [property: JsonPropertyName("token_type")] string? TokenType);
}
