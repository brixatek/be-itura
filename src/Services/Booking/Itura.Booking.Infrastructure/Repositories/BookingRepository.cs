using Itura.Booking.Domain.Entities;
using Itura.Booking.Domain.Repositories;
using Itura.Booking.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Booking.Infrastructure.Repositories;

internal sealed class BookingRepository(BookingDbContext context) : IBookingRepository
{
    public async Task<BookingSession?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.BookingSessions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<PagedResult<BookingSession>> GetByClientUserIdAsync(
        Guid clientUserId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.BookingSessions.Where(b => b.ClientUserId == clientUserId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(b => b.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedResult<BookingSession>(items, total, page, pageSize);
    }

    public async Task<PagedResult<BookingSession>> GetByCoachUserIdAsync(
        Guid coachUserId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.BookingSessions.Where(b => b.CoachUserId == coachUserId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(b => b.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedResult<BookingSession>(items, total, page, pageSize);
    }

    public Task<List<BookingSession>> GetPendingRemindersAsync(DateTime from, DateTime to, bool is24h, CancellationToken ct = default)
    {
        var query = context.BookingSessions
            .Where(b => b.Status == Itura.Booking.Domain.Enums.BookingStatus.Confirmed
                     && b.ScheduledAt >= from && b.ScheduledAt < to);
        query = is24h
            ? query.Where(b => !b.Reminder24hSent)
            : query.Where(b => !b.Reminder1hSent);
        return query.ToListAsync(ct);
    }

    public async Task AddAsync(BookingSession session, CancellationToken ct = default) =>
        await context.BookingSessions.AddAsync(session, ct);

    public void Update(BookingSession session) =>
        context.BookingSessions.Update(session);
}
