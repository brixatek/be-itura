using Itura.Analytics.Domain.Entities;

namespace Itura.Analytics.Domain.Repositories;

public interface IAnalyticsEventRepository
{
    Task AddAsync(AnalyticsEvent analyticsEvent, CancellationToken ct = default);
    Task<(List<AnalyticsEvent> Items, int TotalCount)> GetUserEventsAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<List<(string EventType, int Count)>> GetEventCountsAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
