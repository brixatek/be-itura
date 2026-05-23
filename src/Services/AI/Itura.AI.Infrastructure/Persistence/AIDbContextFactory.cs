using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Itura.AI.Infrastructure.Persistence;

internal sealed class AIDbContextFactory : IDesignTimeDbContextFactory<AIDbContext>
{
    public AIDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<AIDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_ai;Username=postgres;Password=postgres",
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_ai"))
            .Options;
        return new AIDbContext(opts, new NoOpPublisher());
    }

    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
