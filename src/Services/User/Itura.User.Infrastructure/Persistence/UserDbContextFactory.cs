using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Itura.User.Infrastructure.Persistence;

public sealed class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_users;Username=postgres;Password=postgres")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UserDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new UserDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
