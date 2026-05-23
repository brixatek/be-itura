using Itura.Corporate.Application.Common.Interfaces;
using Itura.Corporate.Domain.Events;
using Itura.Corporate.Domain.Repositories;
using Itura.Corporate.Infrastructure.EventHandlers;
using Itura.Corporate.Infrastructure.Persistence;
using Itura.Corporate.Infrastructure.Repositories;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Corporate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCorporateInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CorporateDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("CorporateDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_corporate")));

        services.AddScoped<ICorporateUnitOfWork, UnitOfWork>();
        services.AddScoped<ICorporateAccountRepository, CorporateAccountRepository>();
        services.AddScoped<ICorporateEmployeeRepository, CorporateEmployeeRepository>();

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

        services.AddScoped<INotificationHandler<CorporateAccountCreatedDomainEvent>, CorporateAccountCreatedDomainEventHandler>();

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
