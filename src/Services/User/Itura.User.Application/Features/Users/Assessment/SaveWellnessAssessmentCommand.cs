using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.Assessment;

public sealed record SaveWellnessAssessmentCommand(
    Guid AccountId,
    Dictionary<string, int> Answers) : IRequest<Result<WellnessAssessmentResult>>;

public sealed record WellnessAssessmentResult(int CompositeScore, string RiskLevel, bool CrisisDetected);
