using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.Payment.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Payment.Infrastructure.Repositories;

internal sealed class CoachPayoutRepository(PaymentDbContext context) : ICoachPayoutRepository
{
    public async Task<List<CoachEarning>> GetUnpaidEarningsAsync(Guid coachUserId, CancellationToken ct = default) =>
        await context.CoachEarnings
            .Where(e => e.CoachUserId == coachUserId && !e.IsPaid)
            .ToListAsync(ct);

    public async Task<decimal> GetTotalEarningsAsync(Guid coachUserId, CancellationToken ct = default) =>
        await context.CoachEarnings
            .Where(e => e.CoachUserId == coachUserId)
            .SumAsync(e => e.NetAmount, ct);

    public async Task<decimal> GetUnpaidEarningsTotalAsync(Guid coachUserId, CancellationToken ct = default) =>
        await context.CoachEarnings
            .Where(e => e.CoachUserId == coachUserId && !e.IsPaid)
            .SumAsync(e => e.NetAmount, ct);

    public async Task<decimal> GetUnpaidEarningsCommissionTotalAsync(Guid coachUserId, CancellationToken ct = default) =>
        await context.CoachEarnings
            .Where(e => e.CoachUserId == coachUserId && !e.IsPaid)
            .SumAsync(e => e.CommissionAmount, ct);

    public async Task AddEarningAsync(CoachEarning earning, CancellationToken ct = default) =>
        await context.CoachEarnings.AddAsync(earning, ct);

    public void UpdateEarning(CoachEarning earning) => context.CoachEarnings.Update(earning);

    public async Task<CoachBankAccount?> GetBankAccountAsync(Guid coachUserId, CancellationToken ct = default) =>
        await context.CoachBankAccounts.FirstOrDefaultAsync(b => b.CoachUserId == coachUserId, ct);

    public async Task AddBankAccountAsync(CoachBankAccount account, CancellationToken ct = default) =>
        await context.CoachBankAccounts.AddAsync(account, ct);

    public void UpdateBankAccount(CoachBankAccount account) => context.CoachBankAccounts.Update(account);

    public async Task<PagedResult<CoachPayout>> GetPayoutsAsync(
        Guid coachUserId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.CoachPayouts.Where(p => p.CoachUserId == coachUserId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedResult<CoachPayout>(items, total, page, pageSize);
    }

    public async Task<CoachPayout?> GetPayoutByTransferReferenceAsync(string reference, CancellationToken ct = default) =>
        await context.CoachPayouts.FirstOrDefaultAsync(p => p.TransferReference == reference, ct);

    public async Task AddPayoutAsync(CoachPayout payout, CancellationToken ct = default) =>
        await context.CoachPayouts.AddAsync(payout, ct);

    public void UpdatePayout(CoachPayout payout) => context.CoachPayouts.Update(payout);

    public async Task<List<Guid>> GetCoachesWithUnpaidEarningsAsync(CancellationToken ct = default) =>
        await context.CoachEarnings
            .Where(e => !e.IsPaid)
            .Select(e => e.CoachUserId)
            .Distinct()
            .ToListAsync(ct);
}
