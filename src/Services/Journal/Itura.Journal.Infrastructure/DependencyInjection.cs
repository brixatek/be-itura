using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Domain.Events;
using Itura.Journal.Domain.Repositories;
using Itura.Journal.Infrastructure.EventHandlers;
using Itura.Journal.Infrastructure.Persistence;
using Itura.Journal.Infrastructure.Repositories;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Journal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddJournalInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<JournalDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("JournalDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_journal")));

        services.AddScoped<IJournalUnitOfWork, UnitOfWork>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();

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

        services.AddScoped<INotificationHandler<JournalEntryCreatedDomainEvent>, JournalEntryCreatedDomainEventHandler>();

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
