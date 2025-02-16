using Cashflow.Core;
using Cashflow.Domain.Abstractions.EventHandling;
using MassTransit;

namespace Cashflow.Infrastructure.Messaging;

public class MassTransitEventMediator : IEventMediator
{
    private readonly IBus _bus;

    public MassTransitEventMediator(IBus bus)
    {
        _bus = bus;
    }
    
    public Task NotifyAsync<T>(T @event)
        where T : IEvent
    {
        return _bus.Publish(@event);
    }
}