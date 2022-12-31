using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events;

public abstract class BaseEventBus : IEventBus
{
    public readonly IServiceProvider _serviceProvider;
    public readonly IEventBusSubscriptionManager _subscriptionManager;

    public EventBusConfig _eventBusConfig { get; set; }

    protected BaseEventBus(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
    {
        _eventBusConfig = eventBusConfig;
        _serviceProvider = serviceProvider;
        _subscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
    }

    public virtual string ProcessEventName(string eventName)
    {
        if (_eventBusConfig.DeleteEventPrefix)
        {
            eventName = eventName.TrimStart(_eventBusConfig.EventNamePrefix.ToArray());
        }

        if (_eventBusConfig.DeleteEventSuffix)
        {
            eventName = eventName.TrimEnd(_eventBusConfig.EventNameSuffix.ToArray());
        }

        return eventName;
    }

    public virtual string GetSubName(string eventName)
    {
        return $"{_eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
    }

    public virtual void Dispose()
    {
        _eventBusConfig = null;
        _subscriptionManager.Clear();
    }

    public async Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);

        var processed = false;

        if (_subscriptionManager.HasSubscriptionForEvent(eventName))
        {
            var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);

            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var sub in subscriptions)
                {
                    var handler = _serviceProvider.GetService(sub.HandlerType);

                    if (handler == null) continue;

                    var eventType = _subscriptionManager.GetEventTypeByName($"{_eventBusConfig.EventNamePrefix}{eventName}{_eventBusConfig.EventNameSuffix}");
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }

            processed = true;
        }

        return processed;
    }

    public abstract void Publish(IntegrationEvent @event);

    public abstract void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    public abstract void UnSubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}