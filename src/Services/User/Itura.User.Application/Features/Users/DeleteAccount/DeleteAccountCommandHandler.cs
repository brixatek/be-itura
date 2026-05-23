using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.DeleteAccount;

internal sealed class DeleteAccountCommandHandler(
    IUserProfileRepository profileRepository,
    IUserUnitOfWork unitOfWork)
    : IRequestHandler<DeleteAccountCommand, Result>
{
    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (profile is null)
            return Result.Failure(Error.NotFound("UserProfile", request.AccountId));

        if (profile.IsDeleted)
            return Result.Success();

        profile.Anonymize();
        profileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
