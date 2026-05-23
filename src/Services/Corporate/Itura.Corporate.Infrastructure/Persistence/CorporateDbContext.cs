using Itura.Corporate.Domain.Entities;
using Itura.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Itura.Corporate.Infrastructure.Persistence;

public sealed class CorporateDbContext(DbContextOptions<CorporateDbContext> options, IPublisher publisher)
    : DbContext(options)
{
    public DbSet<CorporateAccount> Accounts => Set<CorporateAccount>();
    public DbSet<CorporateEmployee> Employees => Set<CorporateEmployee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("itura_corporate");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CorporateDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity).ToList();

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
