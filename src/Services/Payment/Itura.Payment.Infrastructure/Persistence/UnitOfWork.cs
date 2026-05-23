using Itura.Payment.Application.Common.Interfaces;

namespace Itura.Payment.Infrastructure.Persistence;

internal sealed class UnitOfWork(PaymentDbContext context) : IPaymentUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
