using Itura.Mood.Application.Common.Interfaces;
using Itura.Mood.Domain.Events;
using Itura.Mood.Domain.Repositories;
using Itura.Mood.Infrastructure.EventHandlers;
using Itura.Mood.Infrastructure.Persistence;
using Itura.Mood.Infrastructure.Repositories;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Mood.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMoodInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Database
        services.AddDbContext<MoodDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("MoodDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_mood")));

        // Unit of work + repository
        services.AddScoped<IMoodUnitOfWork, UnitOfWork>();
        services.AddScoped<IMoodEntryRepository, MoodEntryRepository>();

        // MassTransit — publish MoodLoggedEvent
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

        services.AddScoped<INotificationHandler<MoodEntryCreatedDomainEvent>, MoodEntryCreatedDomainEventHandler>();

        // JWT bearer
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
                        ValidateIssuer = true,
                        ValidIssuer = jwtSection["Issuer"] ?? "itura-auth",
                        ValidateAudience = true,
                        ValidAudience = jwtSection["Audience"] ?? "itura-api",
                        ValidateLifetime = true,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero,
                    };
                });
        }

        services.AddAuthorization();

        return services;
    }
}
