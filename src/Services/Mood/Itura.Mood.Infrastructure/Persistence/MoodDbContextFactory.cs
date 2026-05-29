using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Itura.Mood.Infrastructure.Persistence;

public sealed class MoodDbContextFactory : IDesignTimeDbContextFactory<MoodDbContext>
{
    public MoodDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MoodDbContext>()
            .UseNpgsql("Host=gondola.proxy.rlwy.net;Port=37181;Database=railway;Username=postgres;Password=iRjeiDgqViuqVDRyVboZIUZeWVsYwrEd")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MoodDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new MoodDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
