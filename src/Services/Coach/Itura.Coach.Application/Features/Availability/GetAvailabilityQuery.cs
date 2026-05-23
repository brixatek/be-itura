using Itura.Coach.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

public sealed record GetAvailabilityQuery(Guid CoachUserId) : IRequest<Result<List<AvailabilityBlockDto>>>;
