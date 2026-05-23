using Itura.SharedKernel.Results;
using Itura.User.Application.Features.Users.CompleteOnboarding;
using Itura.User.Application.Features.Users.GetProfile;
using Itura.User.Application.Features.Users.UpdatePreferences;
using Itura.User.Application.Features.Users.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.User.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public sealed class UsersController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("User ID claim missing."));

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await sender.Send(new GetUserProfileQuery(CurrentUserId), ct);
        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var command = new UpdateUserProfileCommand(
            CurrentUserId,
            request.FullName,
            request.AvatarUrl,
            request.Bio,
            request.DateOfBirth,
            request.Gender,
            request.Timezone);

        var result = await sender.Send(command, ct);
        return result.IsSuccess ? Ok() : Problem(result);
    }

    [HttpPost("me/onboarding")]
    public async Task<IActionResult> CompleteOnboarding([FromBody] CompleteOnboardingRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CompleteOnboardingCommand(CurrentUserId, request.WellnessGoals), ct);
        return result.IsSuccess ? Ok() : Problem(result);
    }

    [HttpPut("me/preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request, CancellationToken ct)
    {
        var command = new UpdatePreferencesCommand(
            CurrentUserId,
            request.EmailNotifications,
            request.PushNotifications,
            request.WeeklyDigest,
            request.Theme,
            request.Language);

        var result = await sender.Send(command, ct);
        return result.IsSuccess ? Ok() : Problem(result);
    }

    private IActionResult Problem(Result result)
    {
        var status = result.Error.Code switch
        {
            var c when c.StartsWith("NotFound") => StatusCodes.Status404NotFound,
            var c when c.StartsWith("Conflict") => StatusCodes.Status409Conflict,
            var c when c.StartsWith("Validation") => StatusCodes.Status422UnprocessableEntity,
            var c when c.StartsWith("Unauthorized") => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status400BadRequest
        };
        return Problem(detail: result.Error.Message, statusCode: status, title: result.Error.Code);
    }
}

public sealed record UpdateProfileRequest(
    string FullName,
    string? AvatarUrl,
    string? Bio,
    DateOnly? DateOfBirth,
    string? Gender,
    string Timezone);

public sealed record CompleteOnboardingRequest(List<string> WellnessGoals);

public sealed record UpdatePreferencesRequest(
    bool EmailNotifications,
    bool PushNotifications,
    bool WeeklyDigest,
    string Theme,
    string Language);
