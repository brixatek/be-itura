using Itura.AI.Application.Common.Interfaces;
using Itura.AI.Domain.Repositories;
using Itura.AI.Infrastructure.Persistence;
using Itura.AI.Infrastructure.Repositories;
using Itura.AI.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace Itura.AI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAIInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AIDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("AIDb"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "itura_ai")));

        services.AddScoped<IAIUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRecommendationRepository, UserRecommendationRepository>();

        // MongoDB for conversation history
        var mongoConnStr = config.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
        var mongoClient = new MongoClient(mongoConnStr);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton<IMongoDatabase>(mongoClient.GetDatabase("itura_ai"));
        services.AddScoped<IConversationRepository, ConversationRepository>();

        // Redis for rate limiting & caching
        var redisConn = config.GetConnectionString("Redis") ?? "localhost:6379";
        try { services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn)); }
        catch { services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false")); }
        services.AddScoped<IAIRateLimiter, RedisAIRateLimiter>();
        services.AddScoped<IJournalPromptsCache, RedisJournalPromptsCache>();

        // OpenAI completion service
        var openAiOptions = config.GetSection(OpenAIOptions.Section);
        services.Configure<OpenAIOptions>(openAiOptions);
        services.AddHttpClient("OpenAI", (sp, client) =>
        {
            var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<OpenAIOptions>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
            if (!string.IsNullOrEmpty(opts.ApiKey))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", opts.ApiKey);
        });
        services.AddScoped<IAICompletionService, OpenAICompletionService>();

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
