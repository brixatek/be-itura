using Itura.Content.Application.Common.Interfaces;
using Itura.Content.Domain.Events;
using Itura.Content.Domain.Repositories;
using Itura.Content.Infrastructure.EventHandlers;
using Itura.Content.Infrastructure.Persistence;
using Itura.Content.Infrastructure.Repositories;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Content.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddContentInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ContentDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("ContentDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_content")));

        services.AddScoped<IContentUnitOfWork, UnitOfWork>();
        services.AddScoped<IContentRepository, ContentRepository>();

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

        services.AddScoped<INotificationHandler<ContentItemCreatedDomainEvent>, ContentItemCreatedDomainEventHandler>();
        services.AddScoped<INotificationHandler<ContentItemPublishedDomainEvent>, ContentItemPublishedDomainEventHandler>();

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
