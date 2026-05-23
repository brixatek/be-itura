using Itura.Corporate.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Corporate.Domain.Repositories;

public interface ICorporateEmployeeRepository
{
    Task<CorporateEmployee?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CorporateEmployee?> GetByAccountAndUserAsync(Guid accountId, Guid userId, CancellationToken ct = default);
    Task<PagedResult<CorporateEmployee>> GetByAccountIdAsync(Guid accountId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(CorporateEmployee employee, CancellationToken ct = default);
    void Update(CorporateEmployee employee);
}
