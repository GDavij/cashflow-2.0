using Cashflow.Domain.Abstractions.EventHandling;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Cashflow.Infrastructure.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddScoped<IEventMediator, MassTransitEventMediator>();
        services.AddMassTransit(cfg =>
        {
            cfg.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });

            cfg.AddConsumers(typeof(Domain.DependencyInjection).Assembly);
        });

        return services;
    }
}