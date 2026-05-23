namespace Itura.Auth.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Guid accountId, string email, string role, string jti);
    (string Token, string Hash) GenerateRefreshToken();
    bool ValidateAccessToken(string token, out string? jti);
}
