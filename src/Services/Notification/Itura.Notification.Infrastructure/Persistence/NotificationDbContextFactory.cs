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
            .UseNpgsql("Host=gondola.proxy.rlwy.net;Port=37181;Database=railway;Username=postgres;Password=iRjeiDgqViuqVDRyVboZIUZeWVsYwrEd")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(NotificationDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new NotificationDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
