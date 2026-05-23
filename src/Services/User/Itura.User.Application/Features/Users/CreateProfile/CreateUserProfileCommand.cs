using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.CreateProfile;

public sealed record CreateUserProfileCommand(
    Guid AccountId,
    string Email,
    string FullName,
    string Timezone) : IRequest<Result>;
