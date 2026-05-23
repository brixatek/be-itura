using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.UpdateProfile;

public sealed record UpdateCoachProfileCommand(
    Guid CoachId,
    Guid UserId,
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Specializations,
    IReadOnlyList<string> Languages,
    decimal HourlyRate,
    string Currency,
    string? ProfileImageUrl,
    int YearsOfExperience) : IRequest<Result>;
