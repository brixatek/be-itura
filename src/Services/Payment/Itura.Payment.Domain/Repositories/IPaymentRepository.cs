using Itura.Payment.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Payment.Domain.Repositories;

public interface IPaymentRepository
{
    Task<PaymentRecord?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaymentRecord?> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<PaymentRecord?> GetByReferenceAsync(string reference, CancellationToken ct = default);
    Task<PagedResult<PaymentRecord>> GetByPayerUserIdAsync(Guid payerUserId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(PaymentRecord record, CancellationToken ct = default);
    void Update(PaymentRecord record);
}
