using Itura.Gamification.Application.Common.Interfaces;
using Itura.Gamification.Domain.Events;
using Itura.Gamification.Domain.Repositories;
using Itura.Gamification.Infrastructure.EventHandlers;
using Itura.Gamification.Infrastructure.Persistence;
using Itura.Gamification.Infrastructure.Repositories;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Gamification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGamificationInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<GamificationDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("GamificationDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_gamification")));

        services.AddScoped<IGamificationUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserGamificationProfileRepository, UserGamificationProfileRepository>();

        services.AddMassTransit(x =>
        {
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

        services.AddScoped<INotificationHandler<PointsAwardedDomainEvent>, PointsAwardedDomainEventHandler>();

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
