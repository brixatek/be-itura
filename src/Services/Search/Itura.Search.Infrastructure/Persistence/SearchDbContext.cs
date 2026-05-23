using Itura.Search.Domain.Entities;
using Itura.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Itura.Search.Infrastructure.Persistence;

public sealed class SearchDbContext(DbContextOptions<SearchDbContext> options, IPublisher publisher)
    : DbContext(options)
{
    public DbSet<SearchDocument> Documents => Set<SearchDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("itura_search");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SearchDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0).Select(e => e.Entity).ToList();
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
