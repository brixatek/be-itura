using Itura.Coach.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Coach.Domain.Repositories;

public interface ICoachRepository
{
    Task<CoachProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CoachProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<PagedResult<CoachProfile>> GetActiveAsync(
        int page, int pageSize,
        string? specialization = null,
        string? language = null,
        double? minRating = null,
        decimal? maxHourlyRate = null,
        CancellationToken ct = default);
    Task AddAsync(CoachProfile coach, CancellationToken ct = default);
    void Update(CoachProfile coach);
}
