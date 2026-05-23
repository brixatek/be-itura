using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Itura.Auth.Infrastructure.Persistence;

// Used only by `dotnet ef migrations add` at design time
public sealed class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_auth;Username=postgres;Password=postgres")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AuthDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new AuthDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
