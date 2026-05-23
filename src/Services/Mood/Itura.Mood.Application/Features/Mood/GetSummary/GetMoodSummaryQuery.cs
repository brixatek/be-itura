using Itura.Mood.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Mood.Application.Features.Mood.GetSummary;

public sealed record GetMoodSummaryQuery(Guid UserId, int Days = 30) : IRequest<Result<MoodSummaryDto>>;
