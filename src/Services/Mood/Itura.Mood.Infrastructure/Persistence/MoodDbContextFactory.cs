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
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_mood;Username=postgres;Password=postgres")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MoodDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new MoodDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
