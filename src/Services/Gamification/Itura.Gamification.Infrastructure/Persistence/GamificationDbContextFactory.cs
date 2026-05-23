using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Itura.Gamification.Infrastructure.Persistence;

internal sealed class GamificationDbContextFactory : IDesignTimeDbContextFactory<GamificationDbContext>
{
    public GamificationDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<GamificationDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_gamification;Username=postgres;Password=postgres",
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_gamification"))
            .Options;
        return new GamificationDbContext(opts, new NoOpPublisher());
    }

    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
