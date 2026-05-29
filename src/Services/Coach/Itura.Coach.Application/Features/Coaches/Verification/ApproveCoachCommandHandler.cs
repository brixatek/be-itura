using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.Verification;

internal sealed class ApproveCoachCommandHandler(
    ICoachRepository repository,
    ICoachUnitOfWork unitOfWork,
    ICoachEmailService emailService)
    : IRequestHandler<ApproveCoachCommand, Result>
{
    public async Task<Result> Handle(ApproveCoachCommand request, CancellationToken cancellationToken)
    {
        var coach = await repository.GetByIdAsync(request.CoachProfileId, cancellationToken);
        if (coach is null)
            return Result.Failure(Error.NotFound("Coach.NotFound", "Coach profile not found."));

        var result = coach.Approve(request.AdminId);
        if (result.IsFailure) return result;

        repository.Update(coach);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await emailService.SendApprovalEmailAsync(coach.Email, coach.DisplayName, cancellationToken);

        return Result.Success();
    }
}
