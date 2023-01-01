using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;

namespace EventBus.Factory;

public class EventBusFactory
{
    public static IEventBus Create(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
    {
        return eventBusConfig.EventBusType switch
        {
            EventBusType.AzureServiceBus => new EventBusServiceBus(eventBusConfig, serviceProvider),
            _ => new EventBusRabbitMQ(eventBusConfig, serviceProvider) // Else use RabbitMQ
        };
    }
}