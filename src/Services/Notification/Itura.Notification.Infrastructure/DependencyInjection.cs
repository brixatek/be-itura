using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Domain.Repositories;
using Itura.Notification.Infrastructure.Consumers;
using Itura.Notification.Infrastructure.Persistence;
using Itura.Notification.Infrastructure.Repositories;
using Itura.Notification.Infrastructure.Services;
// Aliases for ambiguous types
using IDeviceTokenRepository = Itura.Notification.Domain.Repositories.IDeviceTokenRepository;
using DeviceTokenRepository = Itura.Notification.Infrastructure.Repositories.DeviceTokenRepository;
using INotificationPreferenceRepository = Itura.Notification.Domain.Repositories.INotificationPreferenceRepository;
using NotificationPreferenceRepository = Itura.Notification.Infrastructure.Repositories.NotificationPreferenceRepository;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Database
        services.AddDbContext<NotificationDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("NotificationDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_notifications")));

        // Unit of work + repository
        services.AddScoped<INotificationUnitOfWork, UnitOfWork>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();

        // Email
        services.Configure<EmailOptions>(config.GetSection(EmailOptions.Section));
        services.AddScoped<IEmailService, EmailService>();

        // Push notifications
        services.AddScoped<IPushNotificationService, FirebasePushService>();

        // MassTransit — consume integration events
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserRegisteredEventConsumer>();
            x.AddConsumer<SendNotificationRequestConsumer>();
            x.AddConsumer<LevelUpEventConsumer>();
            x.AddConsumer<BadgeEarnedEventConsumer>();
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
