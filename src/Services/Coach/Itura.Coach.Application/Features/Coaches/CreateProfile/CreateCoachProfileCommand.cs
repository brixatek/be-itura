using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.CreateProfile;

public sealed record CreateCoachProfileCommand(
    Guid UserId,
    string Email,
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Specializations,
    IReadOnlyList<string> Languages,
    decimal HourlyRate,
    string Currency,
    string? ProfileImageUrl,
    int YearsOfExperience) : IRequest<Result<Guid>>;
