using Itura.SharedKernel.Results;
using Itura.User.Application.Features.Users.Assessment;
using Itura.User.Application.Features.Users.CompleteOnboarding;
using Itura.User.Application.Features.Users.DeleteAccount;
using Itura.User.Application.Features.Users.GetPreferences;
using Itura.User.Application.Features.Users.GetProfile;
using Itura.User.Application.Features.Users.UpdatePreferences;
using Itura.User.Application.Features.Users.UpdateProfile;
using Itura.User.Application.Features.Users.UploadAvatar;
using Itura.User.Application.Features.Gamification;
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

    [HttpPut("me/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { success = false, error = "No file provided." });

        var command = new UploadAvatarCommand(
            CurrentUserId,
            file.OpenReadStream(),
            file.FileName,
            file.ContentType,
            file.Length);

        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Ok(new { success = true, data = new { avatarUrl = result.Value } })
            : Problem(result);
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccount(CancellationToken ct)
    {
        var result = await sender.Send(new DeleteAccountCommand(CurrentUserId), ct);
        return result.IsSuccess ? NoContent() : Problem(result);
    }

    [HttpPost("me/onboarding")]
    public async Task<IActionResult> CompleteOnboarding([FromBody] CompleteOnboardingRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CompleteOnboardingCommand(CurrentUserId, request.WellnessGoals), ct);
        return result.IsSuccess ? Ok() : Problem(result);
    }

    [HttpPost("onboarding/assessment")]
    public async Task<IActionResult> SaveAssessment([FromBody] SaveAssessmentRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SaveWellnessAssessmentCommand(CurrentUserId, request.Answers), ct);
        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    [HttpGet("onboarding/assessment")]
    public async Task<IActionResult> GetAssessment(CancellationToken ct)
    {
        var result = await sender.Send(new GetWellnessAssessmentQuery(CurrentUserId), ct);
        return result.IsSuccess ? Ok(result.Value) : Problem(result);
    }

    [HttpGet("me/preferences")]
    public async Task<IActionResult> GetPreferences(CancellationToken ct)
    {
        var result = await sender.Send(new GetPreferencesQuery(CurrentUserId), ct);
        return result.IsSuccess ? Ok(result.Value) : Problem(result);
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

    [HttpGet("me/badges")]
    public async Task<IActionResult> GetBadges(CancellationToken ct)
    {
        var result = await sender.Send(new GetBadgesQuery(CurrentUserId), ct);
        return result.IsSuccess ? Ok(result.Value) : Problem(result);
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

[ApiController]
[Route("api/v1/gamification")]
[Authorize]
public sealed class GamificationController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("User ID claim missing."));

    [HttpGet("leaderboard")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int count = 50, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetLeaderboardQuery(count), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("my-streak/{streakType}")]
    public async Task<IActionResult> GetStreak(string streakType, CancellationToken ct)
    {
        var result = await sender.Send(new RecordStreakActivityCommand(
            CurrentUserId, streakType, DateOnly.FromDateTime(DateTime.UtcNow)), ct);
        return Ok(new { success = true, data = result.Value });
    }

    private IActionResult Problem(Result result)
    {
        var status = result.Error.Code switch
        {
            var c when c.StartsWith("NotFound") => StatusCodes.Status404NotFound,
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

public sealed record SaveAssessmentRequest(Dictionary<string, int> Answers);
