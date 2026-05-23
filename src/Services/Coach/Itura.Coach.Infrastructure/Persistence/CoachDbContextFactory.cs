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
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_coach;Username=postgres;Password=postgres")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoachDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new CoachDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
