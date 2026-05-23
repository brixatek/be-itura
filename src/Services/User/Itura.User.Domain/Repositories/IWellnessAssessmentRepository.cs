using Itura.User.Domain.Entities;

namespace Itura.User.Domain.Repositories;

public interface IWellnessAssessmentRepository
{
    Task<WellnessAssessment?> GetLatestByUserIdAsync(Guid userProfileId, CancellationToken ct = default);
    Task AddAsync(WellnessAssessment assessment, CancellationToken ct = default);
}
