using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.Verification;

internal sealed class RejectCoachCommandHandler(
    ICoachRepository repository,
    ICoachUnitOfWork unitOfWork,
    ICoachEmailService emailService)
    : IRequestHandler<RejectCoachCommand, Result>
{
    public async Task<Result> Handle(RejectCoachCommand request, CancellationToken cancellationToken)
    {
        var coach = await repository.GetByIdAsync(request.CoachProfileId, cancellationToken);
        if (coach is null)
            return Result.Failure(Error.NotFound("Coach.NotFound", "Coach profile not found."));

        var result = coach.Reject(request.AdminId, request.Reason);
        if (result.IsFailure) return result;

        repository.Update(coach);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await emailService.SendRejectionEmailAsync(coach.Email, coach.DisplayName, request.Reason, cancellationToken);

        return Result.Success();
    }
}
