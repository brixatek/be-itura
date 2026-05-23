using Itura.Booking.Application.Common.Interfaces;

namespace Itura.Booking.Infrastructure.Persistence;

internal sealed class UnitOfWork(BookingDbContext context) : IBookingUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
