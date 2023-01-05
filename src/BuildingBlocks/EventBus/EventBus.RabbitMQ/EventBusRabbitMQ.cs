using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ : BaseEventBus
{
    RabbitMQPersistentConnection _persistentConnection;
    private readonly IConnectionFactory connectionFactory;
    private readonly IModel consumerChannel;

    public EventBusRabbitMQ(EventBusConfig eventBusConfig, IServiceProvider serviceProvider) : base(eventBusConfig, serviceProvider)
    {
        if(eventBusConfig != null)
        {
            var connectionJson = JsonConvert.SerializeObject(eventBusConfig, new JsonSerializerSettings()
            {
                // Self referencing loop detected for property
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connectionJson);
        }
        else
        {
            connectionFactory = new ConnectionFactory();
        }

        _persistentConnection = new RabbitMQPersistentConnection(connectionFactory, eventBusConfig.ConnectionRetryCount);
        consumerChannel = CreateConsumerChannel();
        _subscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
    }

    public override void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_eventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                // Log
            });

        var eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);

        consumerChannel.ExchangeDeclare(exchange: _eventBusConfig.DefaultTopicName, type: "direct"); // Ensure if exchange exists while publishing

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
            var properties = consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2; //persistent

            consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure if queue exists while publishing
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            consumerChannel.QueueBind(queue: GetSubName(eventName),
                    exchange: _eventBusConfig.DefaultTopicName,
                    routingKey: eventName);

            consumerChannel.BasicPublish(
                exchange: _eventBusConfig.DefaultTopicName,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        });
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!_subscriptionManager.HasSubscriptionForEvent(eventName))
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure if queue exists while consuming
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            consumerChannel.QueueBind(queue: GetSubName(eventName),
                exchange: _eventBusConfig.DefaultTopicName,
                routingKey: eventName);
        }

        _subscriptionManager.AddSubscription<T, TH>();
        StartBasicConsume(eventName);
    }

    public override void UnSubscribe<T, TH>()
    {
        _subscriptionManager.RemoveSubscription<T, TH>();
    }

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var channel = _persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange: _eventBusConfig.DefaultTopicName, type: "direct");

        return channel;
    }

    private void StartBasicConsume(string eventName)
    {
        if(consumerChannel != null)
        {
            var consumer = new EventingBasicConsumer(consumerChannel);

            consumer.Received += Consumer_Received;

            consumerChannel.BasicConsume(
                queue: GetSubName(eventName),
                autoAck: false,
                consumer: consumer);
        }
    }

    private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        eventName = ProcessEventName(eventName);
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            // Log
        }

        consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    private void SubscriptionManager_OnEventRemoved(object sender, string eventName)
    {
        eventName = ProcessEventName(eventName);

        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        //using var channel = persistanceConnection.CreateModel();
        consumerChannel.QueueUnbind(queue: eventName,
            exchange: _eventBusConfig.DefaultTopicName,
            routingKey: eventName);

        if (_subscriptionManager.IsEmpty)
        {
            consumerChannel.Close();
        }
    }
}