using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events;
using EventBus.UnitTest.Events.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.UnitTest
{
    public class EventBusTests
    {
        private ServiceCollection _services;

        public EventBusTests()
        {
            _services = new ServiceCollection();
            _services.AddLogging(configure => configure.AddConsole());
        }

        [Fact]
        public void Subscribe_Event_On_RabbitMQ_Test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

            var serviceProvider = _services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }

        [Fact]
        public void Subscribe_Event_On_Azure_Test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });

            var serviceProvider = _services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }

        [Fact]
        public void Send_Message_To_RabbitMQ_Test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

            var serviceProvider = _services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }

        [Fact]
        public void Send_Message_To_Azure_Test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });

            var serviceProvider = _services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }

        [Fact]
        public void Consume_OrderCreated_From_RabbitMQ_Test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

            var serviceProvider = _services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }

        [Fact]
        public void Consume_OrderCreated_From_Azure_Test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });

            var serviceProvider = _services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            Task.Delay(2000).Wait();
        }

        private EventBusConfig GetAzureConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "eCommerceDemoTopicName",
                EventBusType = EventBusType.AzureServiceBus,
                EventNameSuffix = "IntegrationEvent",
                EventBusConnectionString = "YOUR_AZURE_CONNECTION_STRING"
            };
        }

        private EventBusConfig GetRabbitMQConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "eCommerceDemoTopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent",
                //Connection = new ConnectionFactory()  -- DEFAULT SETTINGS
                //{
                //    HostName = "localhost",
                //    Port = 15672,
                //    UserName = "guest",
                //    Password = "guest"
                //}
            };
        }
    }
}