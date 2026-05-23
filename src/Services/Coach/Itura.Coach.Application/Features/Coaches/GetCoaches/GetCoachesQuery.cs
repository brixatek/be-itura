using Itura.Coach.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.GetCoaches;

public sealed record GetCoachesQuery(
    int Page,
    int PageSize,
    string? Specialization = null,
    string? Language = null,
    double? MinRating = null,
    decimal? MaxHourlyRate = null) : IRequest<Result<PagedResult<CoachDto>>>;
