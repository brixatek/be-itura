using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Itura.Media.Infrastructure.Persistence;

internal sealed class MediaDbContextFactory : IDesignTimeDbContextFactory<MediaDbContext>
{
    public MediaDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<MediaDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_media;Username=postgres;Password=postgres",
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_media"))
            .Options;

        return new MediaDbContext(opts, new NoOpPublisher());
    }

    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
