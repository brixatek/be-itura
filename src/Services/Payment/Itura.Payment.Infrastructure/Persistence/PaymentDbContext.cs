using Itura.Payment.Domain.Entities;
using Itura.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Itura.Payment.Infrastructure.Persistence;

public sealed class PaymentDbContext(DbContextOptions<PaymentDbContext> options, IPublisher publisher)
    : DbContext(options)
{
    public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<CoachEarning> CoachEarnings => Set<CoachEarning>();
    public DbSet<CoachBankAccount> CoachBankAccounts => Set<CoachBankAccount>();
    public DbSet<CoachPayout> CoachPayouts => Set<CoachPayout>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("itura_payment");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count != 0)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var aggregate in aggregates)
        {
            var events = aggregate.DomainEvents.ToList();
            aggregate.ClearDomainEvents();
            foreach (var domainEvent in events)
                await publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }
}
