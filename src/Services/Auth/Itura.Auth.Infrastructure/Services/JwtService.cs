using Itura.Auth.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Auth.Infrastructure.Services;

public sealed class JwtOptions
{
    public const string Section = "Jwt";
    public string PrivateKeyPem { get; set; } = string.Empty;
    public string PublicKeyPem { get; set; } = string.Empty;
    public string Issuer { get; set; } = "itura-auth";
    public string Audience { get; set; } = "itura-api";
    public int AccessTokenExpiryMinutes { get; set; } = 15;
}

internal sealed class JwtService(IOptions<JwtOptions> opts) : IJwtService
{
    private readonly JwtOptions _opts = opts.Value;

    public string GenerateAccessToken(Guid accountId, string email, string role, string jti, string? tier = null, string? tenantId = null)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(_opts.PrivateKeyPem);
        var key = new RsaSecurityKey(rsa.ExportParameters(true));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, accountId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        };

        if (!string.IsNullOrEmpty(tier))
            claims.Add(new Claim("tier", tier));
        if (!string.IsNullOrEmpty(tenantId))
            claims.Add(new Claim("tenant_id", tenantId));

        var token = new JwtSecurityToken(
            issuer: _opts.Issuer,
            audience: _opts.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_opts.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string Token, string Hash) GenerateRefreshToken()
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
        return (raw, hash);
    }

    public bool ValidateAccessToken(string token, out string? jti)
    {
        jti = null;
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(_opts.PublicKeyPem);
            var key = new RsaSecurityKey(rsa.ExportParameters(false));

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _opts.Issuer,
                ValidateAudience = true,
                ValidAudience = _opts.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero,
            }, out _);

            jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
