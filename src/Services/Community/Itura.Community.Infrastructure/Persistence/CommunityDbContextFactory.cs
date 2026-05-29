using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Itura.Community.Infrastructure.Persistence;

internal sealed class CommunityDbContextFactory : IDesignTimeDbContextFactory<CommunityDbContext>
{
    public CommunityDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<CommunityDbContext>()
            .UseNpgsql("Host=gondola.proxy.rlwy.net;Port=37181;Database=railway;Username=postgres;Password=iRjeiDgqViuqVDRyVboZIUZeWVsYwrEd",
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_community"))
            .Options;

        return new CommunityDbContext(opts, new NoOpPublisher());
    }

    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
