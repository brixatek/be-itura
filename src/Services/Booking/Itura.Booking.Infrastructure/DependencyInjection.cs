using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Events;
using Itura.Booking.Domain.Repositories;
using Itura.Booking.Infrastructure.BackgroundJobs;
using Itura.Booking.Infrastructure.EventHandlers;
using Itura.Booking.Infrastructure.Persistence;
using Itura.Booking.Infrastructure.Repositories;
using Itura.Booking.Infrastructure.Sagas;
using Itura.Booking.Infrastructure.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace Itura.Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<BookingDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("BookingDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_booking")));

        services.AddScoped<IBookingUnitOfWork, UnitOfWork>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Redis slot reservation
        var redisConn = config.GetConnectionString("Redis") ?? "localhost:6379";
        try
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
        }
        catch
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));
        }
        services.AddScoped<ISlotReservationService, SlotReservationService>();
        services.AddScoped<ICalendarService, IcsCalendarService>();
        services.AddScoped<ICoachAvailabilityCache, CoachAvailabilityCache>();
        services.AddHostedService<SessionReminderJob>();

        services.AddMassTransit(x =>
        {
            x.AddSagaStateMachine<BookingStateMachine, BookingState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    r.ExistingDbContext<BookingDbContext>();
                    r.UsePostgres();
                });

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

        services.AddScoped<INotificationHandler<BookingCreatedDomainEvent>, BookingCreatedDomainEventHandler>();
        services.AddScoped<INotificationHandler<BookingConfirmedDomainEvent>, BookingConfirmedDomainEventHandler>();
        services.AddScoped<INotificationHandler<BookingCancelledDomainEvent>, BookingCancelledDomainEventHandler>();
        services.AddScoped<INotificationHandler<BookingCompletedDomainEvent>, BookingCompletedDomainEventHandler>();

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
