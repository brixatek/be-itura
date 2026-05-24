using Itura.Auth.Application.Features.Auth.ForgotPassword;
using Itura.Auth.Application.Features.Auth.Login;
using Itura.Auth.Application.Features.Auth.Logout;
using Itura.Auth.Application.Features.Auth.Mfa;
using Itura.Auth.Application.Features.Auth.OAuth;
using Itura.Auth.Application.Features.Auth.RefreshToken;
using Itura.Auth.Application.Features.Auth.Register;
using Itura.Auth.Application.Features.Auth.ResetPassword;
using Itura.Auth.Application.Features.Auth.VerifyEmail;
using Itura.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Auth.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.FullName, request.Timezone);
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? StatusCode(201, ApiResponse.Ok(new { userId = result.Value, emailVerificationRequired = true }))
            : Problem(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var command = new LoginCommand(request.Email, request.Password, request.DeviceInfo, ip);
        var result = await sender.Send(command, ct);
        if (result.IsFailure) return Problem(result);

        // MFA challenge — return 200 with mfa_required flag instead of tokens
        if (result.Value.MfaChallenge is not null)
            return Ok(ApiResponse.Ok(result.Value.MfaChallenge));

        return Ok(ApiResponse.Ok(result.Value.Response));
    }

    // ─── MFA endpoints ────────────────────────────────────────────────────────

    [HttpPost("mfa/setup")]
    [Authorize]
    public async Task<IActionResult> MfaSetup(CancellationToken ct)
    {
        var accountId = GetAccountId();
        var result = await sender.Send(new SetupMfaCommand(accountId), ct);
        return result.IsSuccess ? Ok(ApiResponse.Ok(result.Value)) : Problem(result);
    }

    [HttpPost("mfa/verify-setup")]
    [Authorize]
    public async Task<IActionResult> MfaVerifySetup([FromBody] MfaVerifySetupRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        var result = await sender.Send(new VerifyMfaSetupCommand(accountId, request.Code), ct);
        return result.IsSuccess ? Ok(ApiResponse.Ok(result.Value)) : Problem(result);
    }

    [HttpPost("mfa/verify")]
    public async Task<IActionResult> MfaVerify([FromBody] MfaVerifyRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await sender.Send(
            new VerifyMfaCommand(request.AccountId, request.Code, request.UseBackupCode, request.DeviceInfo, ip), ct);
        return result.IsSuccess ? Ok(ApiResponse.Ok(result.Value)) : Problem(result);
    }

    [HttpPost("mfa/disable")]
    [Authorize]
    public async Task<IActionResult> MfaDisable([FromBody] MfaDisableRequest request, CancellationToken ct)
    {
        var accountId = GetAccountId();
        var result = await sender.Send(new DisableMfaCommand(accountId, request.Password), ct);
        return result.IsSuccess ? Ok(ApiResponse.Ok("MFA disabled successfully.")) : Problem(result);
    }

    // ─── OAuth ────────────────────────────────────────────────────────────────

    [HttpPost("oauth/google")]
    public async Task<IActionResult> GoogleOAuth([FromBody] GoogleOAuthRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await sender.Send(new GoogleOAuthCommand(request.Code, request.RedirectUri, request.DeviceInfo, ip), ct);
        return result.IsSuccess ? Ok(ApiResponse.Ok(result.Value)) : Problem(result);
    }

    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new VerifyEmailCommand(request.Token), ct);
        return result.IsSuccess ? Ok(ApiResponse.Ok(result.Value)) : Problem(result);
    }

    [HttpPost("resend-verification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ResendVerificationCommand(request.Email), ct);
        return result.IsSuccess
            ? Ok(ApiResponse.Ok("If that email is registered and unverified, a new verification email has been sent."))
            : Problem(result);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var command = new RefreshTokenCommand(request.RefreshToken, request.DeviceInfo, ip);
        var result = await sender.Send(command, ct);
        return result.IsSuccess ? Ok(ApiResponse.Ok(result.Value)) : Problem(result);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
    {
        var accountId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var jti = User.FindFirstValue("jti") ?? string.Empty;
        var command = new LogoutCommand(accountId, jti, request.RefreshToken, request.LogoutAllDevices);
        await sender.Send(command, ct);
        return Ok(ApiResponse.Ok("Logged out successfully."));
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        await sender.Send(new ForgotPasswordCommand(request.Email), ct);
        return Ok(ApiResponse.Ok("If that email is registered, a reset link has been sent."));
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        var command = new ResetPasswordCommand(request.Token, request.NewPassword, request.ConfirmPassword);
        var result = await sender.Send(command, ct);
        return result.IsSuccess ? Ok(ApiResponse.Ok("Password reset successfully.")) : Problem(result);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private Guid GetAccountId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    private IActionResult Problem(Result result)
    {
        var code = result.Error.Code;
        var status = code switch
        {
            var c when c.Contains("Unauthorized") => StatusCodes.Status401Unauthorized,
            var c when c.Contains("Forbidden") => StatusCodes.Status403Forbidden,
            var c when c.Contains("NotFound") => StatusCodes.Status404NotFound,
            var c when c.Contains("Conflict") || c.Contains("EmailTaken") => StatusCodes.Status409Conflict,
            var c when c.Contains("Validation") => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(status, new
        {
            success = false,
            error = new { code = result.Error.Code, message = result.Error.Message }
        });
    }
}

// ─── Request models ───────────────────────────────────────────────────────────

public sealed record RegisterRequest(string Email, string Password, string FullName, string Timezone);
public sealed record LoginRequest(string Email, string Password, string? DeviceInfo);
public sealed record VerifyEmailRequest(string Token);
public sealed record RefreshTokenRequest(string RefreshToken, string? DeviceInfo);
public sealed record LogoutRequest(string RefreshToken, bool LogoutAllDevices = false);
public sealed record ForgotPasswordRequest(string Email);
public sealed record ResetPasswordRequest(string Token, string NewPassword, string ConfirmPassword);
public sealed record MfaVerifySetupRequest(string Code);
public sealed record MfaVerifyRequest(Guid AccountId, string Code, bool UseBackupCode = false, string? DeviceInfo = null);
public sealed record MfaDisableRequest(string Password);
public sealed record GoogleOAuthRequest(string Code, string RedirectUri, string? DeviceInfo = null);
public sealed record ResendVerificationRequest(string Email);

// ─── Standard response wrapper ────────────────────────────────────────────────

public static class ApiResponse
{
    public static object Ok<T>(T data) => new { success = true, data };
}
