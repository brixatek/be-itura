namespace Itura.Auth.Application.DTOs;

public sealed record AuthTokensDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType = "Bearer");

public sealed record AuthUserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string Tier,
    bool OnboardingCompleted,
    int WellnessLevel);

public sealed record LoginResponseDto(
    AuthTokensDto Tokens,
    AuthUserDto User);

public sealed record MfaRequiredDto(
    bool MfaRequired,
    Guid AccountId,
    string Message = "MFA verification required.");
