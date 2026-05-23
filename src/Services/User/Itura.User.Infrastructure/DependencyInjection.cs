using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Repositories;
using Itura.User.Infrastructure.BackgroundJobs;
using Itura.User.Infrastructure.Consumers;
using Itura.User.Infrastructure.Persistence;
using Itura.User.Infrastructure.Repositories;
using Itura.User.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace Itura.User.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUserInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Database
        services.AddDbContext<UserDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("UserDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_users")));

        // Unit of work + repositories
        services.AddScoped<IUserUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IWellnessAssessmentRepository, WellnessAssessmentRepository>();
        services.AddScoped<IXpRepository, XpRepository>();
        services.AddScoped<IStreakRepository, StreakRepository>();
        services.AddScoped<IBadgeRepository, BadgeRepository>();

        // Redis
        var redisConn = config.GetConnectionString("Redis") ?? "localhost:6379";
        try
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
        }
        catch
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));
        }
        services.AddStackExchangeRedisCache(opts => opts.Configuration = redisConn);
        services.AddScoped<IPreferencesCache, PreferencesCache>();
        services.AddScoped<ILeaderboardCache, LeaderboardCache>();

        // Background jobs
        services.AddHostedService<CheckStreakAtRiskJob>();

        // MassTransit — consume UserRegisteredEvent from Auth
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserRegisteredEventConsumer>();
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

        // JWT bearer — validates tokens issued by Auth service
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
