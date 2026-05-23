using Itura.Coach.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.GetMyProfile;

public sealed record GetMyCoachProfileQuery(Guid UserId) : IRequest<Result<CoachDto>>;
