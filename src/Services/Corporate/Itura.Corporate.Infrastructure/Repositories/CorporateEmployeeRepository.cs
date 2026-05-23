using Itura.Corporate.Domain.Entities;
using Itura.Corporate.Domain.Repositories;
using Itura.Corporate.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Corporate.Infrastructure.Repositories;

internal sealed class CorporateEmployeeRepository(CorporateDbContext context) : ICorporateEmployeeRepository
{
    public async Task<CorporateEmployee?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Employees.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<CorporateEmployee?> GetByAccountAndUserAsync(Guid accountId, Guid userId, CancellationToken ct = default) =>
        await context.Employees.FirstOrDefaultAsync(e => e.CorporateAccountId == accountId && e.UserId == userId, ct);

    public async Task<PagedResult<CorporateEmployee>> GetByAccountIdAsync(Guid accountId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Employees.Where(e => e.CorporateAccountId == accountId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(e => e.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<CorporateEmployee>(items, total, page, pageSize);
    }

    public async Task AddAsync(CorporateEmployee employee, CancellationToken ct = default) =>
        await context.Employees.AddAsync(employee, ct);

    public void Update(CorporateEmployee employee) => context.Employees.Update(employee);
}
