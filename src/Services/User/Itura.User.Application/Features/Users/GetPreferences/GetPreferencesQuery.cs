using Itura.SharedKernel.Results;
using Itura.User.Application.DTOs;
using MediatR;

namespace Itura.User.Application.Features.Users.GetPreferences;

public sealed record GetPreferencesQuery(Guid AccountId) : IRequest<Result<UserPreferencesDto>>;
