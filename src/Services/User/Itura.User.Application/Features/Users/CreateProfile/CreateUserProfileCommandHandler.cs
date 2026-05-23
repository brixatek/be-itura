using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.CreateProfile;

internal sealed class CreateUserProfileCommandHandler(
    IUserProfileRepository profileRepository,
    IUserUnitOfWork unitOfWork) : IRequestHandler<CreateUserProfileCommand, Result>
{
    public async Task<Result> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        if (await profileRepository.ExistsByIdAsync(request.AccountId, cancellationToken))
            return Result.Success(); // idempotent — already created

        var profile = UserProfile.Create(request.AccountId, request.Email, request.FullName, request.Timezone);
        await profileRepository.AddAsync(profile, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
