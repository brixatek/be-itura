using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Itura.Coach.Infrastructure.Persistence;

public sealed class CoachDbContextFactory : IDesignTimeDbContextFactory<CoachDbContext>
{
    public CoachDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CoachDbContext>()
            .UseNpgsql("Host=gondola.proxy.rlwy.net;Port=37181;Database=railway;Username=postgres;Password=iRjeiDgqViuqVDRyVboZIUZeWVsYwrEd")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoachDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new CoachDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
