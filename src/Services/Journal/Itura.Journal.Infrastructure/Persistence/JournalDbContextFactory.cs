using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Itura.Journal.Infrastructure.Persistence;

public sealed class JournalDbContextFactory : IDesignTimeDbContextFactory<JournalDbContext>
{
    public JournalDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<JournalDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=itura_journal;Username=postgres;Password=postgres")
            .Options;

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(JournalDbContext).Assembly));
        var sp = services.BuildServiceProvider();

        return new JournalDbContext(options, sp.GetRequiredService<IPublisher>());
    }
}
