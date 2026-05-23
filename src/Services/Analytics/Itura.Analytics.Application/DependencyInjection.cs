using FluentValidation;
using Itura.Analytics.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Itura.Analytics.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAnalyticsApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
