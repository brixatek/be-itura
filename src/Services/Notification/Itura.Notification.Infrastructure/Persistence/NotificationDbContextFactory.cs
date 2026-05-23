using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Itura.Notification.Infrastructure.Persistence;

public sealed class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_notifications;Username=postgres;Password=postgres")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(NotificationDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new NotificationDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
