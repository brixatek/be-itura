using FluentValidation;
using Itura.Gamification.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Itura.Gamification.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGamificationApplication(this IServiceCollection services)
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
