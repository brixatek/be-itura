using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .Enrich.FromLogContext()
       .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHealthChecks();

builder.Services.AddCors(opts => opts.AddPolicy("AllowFrontend", policy =>
    policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"])
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials()));

var app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    // SwaggerUI only — the gateway itself has no spec.
    // Each endpoint below is a YARP-proxied path to the real service's swagger.json.
    app.UseSwaggerUI(opts =>
    {
        opts.RoutePrefix = "swagger";
        opts.DocumentTitle = "Itura API";
        opts.SwaggerEndpoint("/swagger-json/auth/swagger/v1/swagger.json",          "Auth");
        opts.SwaggerEndpoint("/swagger-json/user/swagger/v1/swagger.json",          "User");
        opts.SwaggerEndpoint("/swagger-json/ai/swagger/v1/swagger.json",            "AI (Sera)");
        opts.SwaggerEndpoint("/swagger-json/mood/swagger/v1/swagger.json",          "Mood");
        opts.SwaggerEndpoint("/swagger-json/journal/swagger/v1/swagger.json",       "Journal");
        opts.SwaggerEndpoint("/swagger-json/coach/swagger/v1/swagger.json",         "Coach");
        opts.SwaggerEndpoint("/swagger-json/booking/swagger/v1/swagger.json",       "Booking");
        opts.SwaggerEndpoint("/swagger-json/payment/swagger/v1/swagger.json",       "Payment");
        opts.SwaggerEndpoint("/swagger-json/notification/swagger/v1/swagger.json",  "Notification");
        opts.SwaggerEndpoint("/swagger-json/community/swagger/v1/swagger.json",     "Community");
        opts.SwaggerEndpoint("/swagger-json/content/swagger/v1/swagger.json",       "Content");
        opts.SwaggerEndpoint("/swagger-json/media/swagger/v1/swagger.json",         "Media");
        opts.SwaggerEndpoint("/swagger-json/corporate/swagger/v1/swagger.json",     "Corporate");
        opts.SwaggerEndpoint("/swagger-json/gamification/swagger/v1/swagger.json",  "Gamification");
        opts.SwaggerEndpoint("/swagger-json/analytics/swagger/v1/swagger.json",     "Analytics");
        opts.SwaggerEndpoint("/swagger-json/search/swagger/v1/swagger.json",        "Search");
        opts.EnableDeepLinking();
        opts.DisplayRequestDuration();
    });
}

app.MapReverseProxy();
app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok(new
{
    service = "Itura Gateway",
    version = "v1",
    status = "running",
    docs = "/swagger"
}));

app.Run();
