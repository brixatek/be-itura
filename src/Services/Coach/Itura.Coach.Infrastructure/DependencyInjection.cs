using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Events;
using Itura.Coach.Domain.Repositories;
using Itura.Coach.Infrastructure.EventHandlers;
using Itura.Coach.Infrastructure.Persistence;
using Itura.Coach.Infrastructure.Repositories;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Coach.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCoachInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CoachDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("CoachDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_coach")));

        services.AddScoped<ICoachUnitOfWork, UnitOfWork>();
        services.AddScoped<ICoachRepository, CoachRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<ICoachEmailService, Itura.Coach.Infrastructure.Services.CoachEmailService>();

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

        services.AddScoped<INotificationHandler<CoachProfileCreatedDomainEvent>, CoachProfileCreatedDomainEventHandler>();
        services.AddScoped<INotificationHandler<CoachRatingUpdatedDomainEvent>, CoachRatingUpdatedDomainEventHandler>();

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
