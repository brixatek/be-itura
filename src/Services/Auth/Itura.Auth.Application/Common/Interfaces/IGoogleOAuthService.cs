namespace Itura.Auth.Application.Common.Interfaces;

public interface IGoogleOAuthService
{
    Task<GoogleUserInfo?> ExchangeCodeAsync(string code, string redirectUri, CancellationToken ct = default);
}

public sealed record GoogleUserInfo(
    string ProviderId,
    string Email,
    string? FullName,
    string? PictureUrl,
    bool EmailVerified);
