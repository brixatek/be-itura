using Itura.Media.Application.Common.Interfaces;
using Itura.Media.Domain.Events;
using Itura.Media.Domain.Repositories;
using Itura.Media.Infrastructure.EventHandlers;
using Itura.Media.Infrastructure.Persistence;
using Itura.Media.Infrastructure.Repositories;
using Itura.Media.Infrastructure.Storage;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Media.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<MediaDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("MediaDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_media")));

        services.AddScoped<IMediaUnitOfWork, UnitOfWork>();
        services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

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

        services.AddScoped<INotificationHandler<MediaAssetUploadedDomainEvent>, MediaAssetUploadedDomainEventHandler>();

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
