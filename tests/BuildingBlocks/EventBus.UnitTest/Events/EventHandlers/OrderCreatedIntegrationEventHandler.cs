using EventBus.Base.Abstraction;

namespace EventBus.UnitTest.Events.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public Task Handle(OrderCreatedIntegrationEvent @event)
    {
        Console.WriteLine("Handle method worked with id:" + @event.Id);

        return Task.CompletedTask;
    }
}