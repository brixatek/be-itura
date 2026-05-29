using Itura.Payment.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Payment.Domain.Repositories;

public interface ICoachPayoutRepository
{
    Task<List<CoachEarning>> GetUnpaidEarningsAsync(Guid coachUserId, CancellationToken ct = default);
    Task<decimal> GetTotalEarningsAsync(Guid coachUserId, CancellationToken ct = default);
    Task<decimal> GetUnpaidEarningsTotalAsync(Guid coachUserId, CancellationToken ct = default);
    Task<decimal> GetUnpaidEarningsCommissionTotalAsync(Guid coachUserId, CancellationToken ct = default);
    Task AddEarningAsync(CoachEarning earning, CancellationToken ct = default);
    void UpdateEarning(CoachEarning earning);

    Task<CoachBankAccount?> GetBankAccountAsync(Guid coachUserId, CancellationToken ct = default);
    Task AddBankAccountAsync(CoachBankAccount account, CancellationToken ct = default);
    void UpdateBankAccount(CoachBankAccount account);

    Task<PagedResult<CoachPayout>> GetPayoutsAsync(Guid coachUserId, int page, int pageSize, CancellationToken ct = default);
    Task<CoachPayout?> GetPayoutByTransferReferenceAsync(string reference, CancellationToken ct = default);
    Task AddPayoutAsync(CoachPayout payout, CancellationToken ct = default);
    void UpdatePayout(CoachPayout payout);

    Task<List<Guid>> GetCoachesWithUnpaidEarningsAsync(CancellationToken ct = default);
}
