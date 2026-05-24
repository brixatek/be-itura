using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Repositories;
using Itura.Community.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Community.Infrastructure.Repositories;

internal sealed class PostReportRepository(CommunityDbContext context) : IPostReportRepository
{
    public Task<bool> ExistsAsync(Guid postId, Guid reporterUserId, CancellationToken ct = default) =>
        context.PostReports.AnyAsync(r => r.PostId == postId && r.ReporterUserId == reporterUserId, ct);

    public async Task AddAsync(PostReport report, CancellationToken ct = default) =>
        await context.PostReports.AddAsync(report, ct);
}
