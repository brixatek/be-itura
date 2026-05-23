using Itura.SharedKernel.Domain;
using Itura.User.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Itura.User.Infrastructure.Persistence;

public sealed class UserDbContext(DbContextOptions<UserDbContext> options, IPublisher publisher) : DbContext(options)
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<WellnessGoal> WellnessGoals => Set<WellnessGoal>();
    public DbSet<WellnessAssessment> WellnessAssessments => Set<WellnessAssessment>();
    public DbSet<XpTransaction> XpTransactions => Set<XpTransaction>();
    public DbSet<UserStreak> UserStreaks => Set<UserStreak>();
    public DbSet<BadgeDefinition> BadgeDefinitions => Set<BadgeDefinition>();
    public DbSet<BadgeEarned> BadgesEarned => Set<BadgeEarned>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("itura_users");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
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
