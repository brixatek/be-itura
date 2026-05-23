using Itura.Media.Domain.Entities;
using Itura.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Itura.Media.Infrastructure.Persistence;

public sealed class MediaDbContext(DbContextOptions<MediaDbContext> options, IPublisher publisher)
    : DbContext(options)
{
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("itura_media");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
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
