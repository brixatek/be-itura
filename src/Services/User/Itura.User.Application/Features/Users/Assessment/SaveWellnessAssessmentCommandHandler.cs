using Itura.Contracts.AI;
using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Itura.User.Application.Features.Users.Assessment;

internal sealed class SaveWellnessAssessmentCommandHandler(
    IUserProfileRepository profileRepository,
    IWellnessAssessmentRepository assessmentRepository,
    IUserUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    ILogger<SaveWellnessAssessmentCommandHandler> logger)
    : IRequestHandler<SaveWellnessAssessmentCommand, Result<WellnessAssessmentResult>>
{
    public async Task<Result<WellnessAssessmentResult>> Handle(
        SaveWellnessAssessmentCommand request, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (profile is null)
            return Result.Failure<WellnessAssessmentResult>(Error.NotFound("UserProfile", request.AccountId));

        if (request.Answers.Count == 0)
            return Result.Failure<WellnessAssessmentResult>(
                Error.Validation("Assessment.NoAnswers", "At least one answer is required."));

        var assessment = WellnessAssessment.Create(request.AccountId, request.Answers);
        await assessmentRepository.AddAsync(assessment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var isCrisis = assessment.RiskLevel == "Crisis";
        if (isCrisis)
        {
            logger.LogWarning("Crisis risk level detected for user {UserId} — score {Score}", request.AccountId, assessment.CompositeScore);
            await publishEndpoint.Publish(new CrisisDetectedEvent(
                request.AccountId,
                $"Assessment score {assessment.CompositeScore} — risk level Crisis",
                "Assessment",
                DateTime.UtcNow), cancellationToken);
        }

        return Result.Success(new WellnessAssessmentResult(assessment.CompositeScore, assessment.RiskLevel, isCrisis));
    }
}
