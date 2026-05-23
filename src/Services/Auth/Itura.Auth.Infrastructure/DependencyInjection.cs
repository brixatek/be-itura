using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Events;
using Itura.Auth.Domain.Repositories;
using Itura.Auth.Infrastructure.EventHandlers;
using Itura.Auth.Infrastructure.Persistence;
using Itura.Auth.Infrastructure.Repositories;
using Itura.Auth.Infrastructure.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace Itura.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Database
        services.AddDbContext<AuthDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("AuthDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_auth")));

        // Unit of work + repositories
        services.AddScoped<IAuthUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IAuthAuditLogRepository, AuthAuditLogRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(config.GetConnectionString("Redis") ?? "localhost:6379"));
        services.AddScoped<ICacheService, RedisCacheService>();

        // JWT
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.Section));
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITotpService, TotpService>();

        // Google OAuth
        services.Configure<GoogleOAuthOptions>(config.GetSection(GoogleOAuthOptions.Section));
        services.AddHttpClient("Google");
        services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();

        // JWT bearer authentication
        var jwtSection = config.GetSection(JwtOptions.Section);
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

        services.AddAuthorization(opts =>
        {
            opts.AddPolicy("RequireAdmin", policy =>
                policy.RequireClaim("role", "Admin", "SuperAdmin"));
        });

        // Email
        services.Configure<EmailOptions>(config.GetSection(EmailOptions.Section));
        services.AddScoped<IEmailService, EmailService>();

        // MassTransit — publish integration events to RabbitMQ
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

        // Domain event handler registered manually so MediatR picks it up from Infrastructure assembly
        services.AddScoped<INotificationHandler<AccountRegisteredDomainEvent>, AccountRegisteredDomainEventHandler>();

        return services;
    }
}
