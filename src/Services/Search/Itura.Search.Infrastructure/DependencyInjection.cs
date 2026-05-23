using Itura.Search.Application.Common.Interfaces;
using Itura.Search.Domain.Repositories;
using Itura.Search.Infrastructure.Persistence;
using Itura.Search.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Search.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSearchInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<SearchDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("SearchDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_search")));

        services.AddScoped<ISearchUnitOfWork, UnitOfWork>();
        services.AddScoped<ISearchDocumentRepository, SearchDocumentRepository>();

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
