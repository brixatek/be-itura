using Itura.Booking.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Booking.Domain.Repositories;

public interface IBookingRepository
{
    Task<BookingSession?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<BookingSession>> GetByClientUserIdAsync(Guid clientUserId, int page, int pageSize, CancellationToken ct = default);
    Task<PagedResult<BookingSession>> GetByCoachUserIdAsync(Guid coachUserId, int page, int pageSize, CancellationToken ct = default);
    Task<List<BookingSession>> GetPendingRemindersAsync(DateTime from, DateTime to, bool is24h, CancellationToken ct = default);
    Task<List<DateTime>> GetBookedSlotsForDayAsync(Guid coachUserId, DateOnly date, CancellationToken ct = default);
    Task AddAsync(BookingSession session, CancellationToken ct = default);
    void Update(BookingSession session);
}
