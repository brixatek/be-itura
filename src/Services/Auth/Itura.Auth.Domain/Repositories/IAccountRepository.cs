using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Enums;
using Itura.SharedKernel.Results;

namespace Itura.Auth.Domain.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Account?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<Account?> GetByVerifyTokenAsync(string token, CancellationToken ct = default);
    Task<PagedResult<Account>> SearchAsync(string? search, AccountStatus? status, UserRole? role, DateTime? registeredFrom, DateTime? registeredTo, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(AccountStatus? status, CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
    void Update(Account account);
}
