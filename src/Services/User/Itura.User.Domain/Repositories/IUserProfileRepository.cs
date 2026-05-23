using Itura.User.Domain.Entities;

namespace Itura.User.Domain.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default);
    void Update(UserProfile profile);
}
