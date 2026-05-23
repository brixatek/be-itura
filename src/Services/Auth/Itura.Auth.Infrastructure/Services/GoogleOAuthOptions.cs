namespace Itura.Auth.Infrastructure.Services;

public sealed class GoogleOAuthOptions
{
    public const string Section = "Google";
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string TokenEndpoint { get; init; } = "https://oauth2.googleapis.com/token";
    public string JwksUri { get; init; } = "https://www.googleapis.com/oauth2/v3/certs";
}
