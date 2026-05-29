using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Domain.Events;
using Itura.Payment.Domain.Repositories;
using Itura.Payment.Infrastructure.BackgroundJobs;
using Itura.Payment.Infrastructure.EventHandlers;
using Itura.Payment.Infrastructure.Persistence;
using Itura.Payment.Infrastructure.Repositories;
using Itura.Payment.Infrastructure.Services;
using IWalletRepository = Itura.Payment.Domain.Repositories.IWalletRepository;
using WalletRepository = Itura.Payment.Infrastructure.Repositories.WalletRepository;
using ICoachPayoutRepository = Itura.Payment.Domain.Repositories.ICoachPayoutRepository;
using CoachPayoutRepository = Itura.Payment.Infrastructure.Repositories.CoachPayoutRepository;
using IFieldEncryptionService = Itura.Payment.Application.Common.Interfaces.IFieldEncryptionService;
using FieldEncryptionService = Itura.Payment.Infrastructure.Services.FieldEncryptionService;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<PaymentDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("PaymentDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_payment")));

        services.AddScoped<IPaymentUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ICoachPayoutRepository, CoachPayoutRepository>();
        services.AddScoped<IFieldEncryptionService, FieldEncryptionService>();

        // Background jobs
        services.AddHostedService<WeeklyPayoutJob>();

        // Paystack
        services.Configure<PaystackOptions>(config.GetSection(PaystackOptions.Section));
        services.AddHttpClient("Paystack", (sp, client) =>
        {
            var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PaystackOptions>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
            if (!string.IsNullOrEmpty(opts.SecretKey))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {opts.SecretKey}");
        });
        services.AddScoped<IPaystackService, PaystackService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BookingCompletedEventConsumer>();
            x.AddConsumer<UserRegisteredEventConsumer>();
            x.AddConsumer<BookingRefundRequestedEventConsumer>();

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

        services.AddScoped<INotificationHandler<PaymentInitiatedDomainEvent>, PaymentInitiatedDomainEventHandler>();
        services.AddScoped<INotificationHandler<PaymentCompletedDomainEvent>, PaymentCompletedDomainEventHandler>();
        services.AddScoped<INotificationHandler<PaymentFailedDomainEvent>, PaymentFailedDomainEventHandler>();
        services.AddScoped<INotificationHandler<PaymentRefundedDomainEvent>, PaymentRefundedDomainEventHandler>();

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
