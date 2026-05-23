using Itura.SharedKernel.Results;
using Itura.User.Application.DTOs;
using MediatR;

namespace Itura.User.Application.Features.Users.GetProfile;

public sealed record GetUserProfileQuery(Guid AccountId) : IRequest<Result<UserProfileDto>>;
