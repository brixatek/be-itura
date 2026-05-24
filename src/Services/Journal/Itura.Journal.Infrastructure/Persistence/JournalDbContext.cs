using Itura.Journal.Domain.Entities;
using Itura.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Itura.Journal.Infrastructure.Persistence;

public sealed class JournalDbContext(DbContextOptions<JournalDbContext> options, IPublisher publisher)
    : DbContext(options)
{
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalCoachShare> JournalCoachShares => Set<JournalCoachShare>();
    public DbSet<JournalTemplate> JournalTemplates => Set<JournalTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("itura_journal");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JournalDbContext).Assembly);
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
