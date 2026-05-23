using Itura.SharedKernel.Results;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.Assessment;

internal sealed class GetWellnessAssessmentQueryHandler(
    IWellnessAssessmentRepository assessmentRepository)
    : IRequestHandler<GetWellnessAssessmentQuery, Result<WellnessAssessmentResult>>
{
    public async Task<Result<WellnessAssessmentResult>> Handle(
        GetWellnessAssessmentQuery request, CancellationToken cancellationToken)
    {
        var assessment = await assessmentRepository.GetLatestByUserIdAsync(request.AccountId, cancellationToken);
        if (assessment is null)
            return Result.Failure<WellnessAssessmentResult>(
                Error.NotFound("Assessment.NotFound", "No assessment found for this user."));

        return Result.Success(new WellnessAssessmentResult(
            assessment.CompositeScore, assessment.RiskLevel, assessment.RiskLevel == "Crisis"));
    }
}
