using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Domain.Events;
using Itura.Community.Domain.Repositories;
using Itura.Community.Infrastructure.EventHandlers;
using Itura.Community.Infrastructure.Persistence;
using Itura.Community.Infrastructure.Repositories;
using Itura.Community.Infrastructure.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Itura.Community.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCommunityInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CommunityDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("CommunityDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_community")));

        services.AddScoped<ICommunityUnitOfWork, UnitOfWork>();
        services.AddScoped<ICommunityPostRepository, CommunityPostRepository>();
        services.AddScoped<IPostCommentRepository, PostCommentRepository>();
        services.AddScoped<IPostReportRepository, PostReportRepository>();
        services.AddScoped<IPostReactionRepository, PostReactionRepository>();
        services.AddScoped<IContentModerationService, ContentModerationService>();

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

        services.AddScoped<INotificationHandler<CommunityPostCreatedDomainEvent>, CommunityPostCreatedDomainEventHandler>();

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
