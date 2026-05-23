using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.Payment.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Payment.Infrastructure.Repositories;

internal sealed class PaymentRepository(PaymentDbContext context) : IPaymentRepository
{
    public async Task<PaymentRecord?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.PaymentRecords
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PaymentRecord?> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default) =>
        await context.PaymentRecords.FirstOrDefaultAsync(p => p.BookingId == bookingId, ct);

    public async Task<PaymentRecord?> GetByReferenceAsync(string reference, CancellationToken ct = default) =>
        await context.PaymentRecords.FirstOrDefaultAsync(p => p.TransactionReference == reference, ct);

    public async Task<PagedResult<PaymentRecord>> GetByPayerUserIdAsync(
        Guid payerUserId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.PaymentRecords.Where(p => p.PayerUserId == payerUserId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedResult<PaymentRecord>(items, total, page, pageSize);
    }

    public async Task AddAsync(PaymentRecord record, CancellationToken ct = default) =>
        await context.PaymentRecords.AddAsync(record, ct);

    public void Update(PaymentRecord record) =>
        context.PaymentRecords.Update(record);
}
