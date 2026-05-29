using Itura.Coach.Application.DTOs;

namespace Itura.Coach.Application.Common.Interfaces;

public interface ISlotCache
{
    Task<List<TimeSlotDto>?> GetAsync(Guid coachUserId, DateOnly date, string viewerTimezone, CancellationToken ct = default);
    Task SetAsync(Guid coachUserId, DateOnly date, string viewerTimezone, List<TimeSlotDto> slots, CancellationToken ct = default);
    Task InvalidateAsync(Guid coachUserId, CancellationToken ct = default);
}
