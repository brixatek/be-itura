using Itura.Auth.Domain.Entities;

namespace Itura.Auth.Domain.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Account?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<Account?> GetByVerifyTokenAsync(string token, CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
    void Update(Account account);
}
