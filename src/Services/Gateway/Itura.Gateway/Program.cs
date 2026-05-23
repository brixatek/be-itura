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
app.MapReverseProxy();
app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok(new
{
    service = "Itura Gateway",
    version = "v1",
    status = "running"
}));

app.Run();
