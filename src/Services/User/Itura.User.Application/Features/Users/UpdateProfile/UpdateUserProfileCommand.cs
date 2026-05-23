using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.UpdateProfile;

public sealed record UpdateUserProfileCommand(
    Guid AccountId,
    string FullName,
    string? AvatarUrl,
    string? Bio,
    DateOnly? DateOfBirth,
    string? Gender,
    string Timezone) : IRequest<Result>;
