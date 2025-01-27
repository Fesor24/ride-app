using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Ridely.Application.Abstractions.Behaviors;


namespace Ridely.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        AddAutoMapper(services);
        AddFluentValidation(services);
        AddMediatr(services);

        return services;
    }

    private static void AddAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    private static void AddFluentValidation(IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void AddMediatr(IServiceCollection services)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            opt.AddOpenBehavior(typeof(ValidationBehavior<,>));

            opt.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
    }
}
