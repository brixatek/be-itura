using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using Itura.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.User.Infrastructure.Repositories;

internal sealed class WellnessAssessmentRepository(UserDbContext context) : IWellnessAssessmentRepository
{
    public Task<WellnessAssessment?> GetLatestByUserIdAsync(Guid userProfileId, CancellationToken ct = default) =>
        context.WellnessAssessments
            .Where(a => a.UserProfileId == userProfileId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(WellnessAssessment assessment, CancellationToken ct = default) =>
        await context.WellnessAssessments.AddAsync(assessment, ct);
}
