using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.Assessment;

public sealed record GetWellnessAssessmentQuery(Guid AccountId) : IRequest<Result<WellnessAssessmentResult>>;
