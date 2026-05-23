using Itura.Analytics.Application.Common.Interfaces;
using Itura.Analytics.Domain.Repositories;
using Itura.Analytics.Infrastructure.Consumers;
using Itura.Analytics.Infrastructure.Persistence;
using Itura.Analytics.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Analytics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAnalyticsInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AnalyticsDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("AnalyticsDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_analytics")));

        services.AddScoped<IAnalyticsUnitOfWork, UnitOfWork>();
        services.AddScoped<IAnalyticsEventRepository, AnalyticsEventRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserRegisteredConsumer>();
            x.AddConsumer<BookingCreatedConsumer>();
            x.AddConsumer<BookingCompletedConsumer>();
            x.AddConsumer<BookingCancelledConsumer>();
            x.AddConsumer<PaymentCompletedConsumer>();
            x.AddConsumer<CommunityPostCreatedConsumer>();
            x.AddConsumer<MoodLoggedConsumer>();
            x.AddConsumer<JournalEntryCreatedConsumer>();
            x.AddConsumer<PointsAwardedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var host = config.GetConnectionString("RabbitMQ") ?? "localhost";
                cfg.Host(host, h =>
                {
                    h.Username(config["RabbitMQ:Username"] ?? "guest");
                    h.Password(config["RabbitMQ:Password"] ?? "guest");
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        var jwtSection = config.GetSection("Jwt");
        var publicKeyPem = jwtSection["PublicKeyPem"] ?? string.Empty;
        if (!string.IsNullOrEmpty(publicKeyPem))
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            var key = new RsaSecurityKey(rsa.ExportParameters(false));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, ValidIssuer = jwtSection["Issuer"] ?? "itura-auth",
                        ValidateAudience = true, ValidAudience = jwtSection["Audience"] ?? "itura-api",
                        ValidateLifetime = true, IssuerSigningKey = key, ClockSkew = TimeSpan.Zero
                    };
                });
        }
        services.AddAuthorization();
        return services;
    }
}
