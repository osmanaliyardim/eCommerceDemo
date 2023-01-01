using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ;

public class RabbitMQPersistentConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly int retryCount;
    private IConnection connection;
    private object lock_object = new object();
    private bool disposed = false;

    public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
    {
        _connectionFactory = connectionFactory;
        this.retryCount = retryCount;
    }

    public bool IsConnected => connection != null && connection.IsOpen;

    public IModel CreateModel()
    {
        return connection.CreateModel();
    }

    public void Dispose()
    {
        disposed = true;
        connection.Dispose();
    }

    public bool TryConnect()
    {
        lock (lock_object)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => {});

            policy.Execute(() =>
            {
                connection = _connectionFactory.CreateConnection();
            });

            if (IsConnected)
            {
                connection.ConnectionShutdown += Connection_ConnectionShutDown;
                connection.CallbackException += Connection_CallbackException;
                connection.ConnectionBlocked += Connection_ConnectionBlocked;

                // Log

                return true;
            }

            return false;
        }
    }

    private void Connection_ConnectionShutDown(object sender, ShutdownEventArgs e)
    {
        // Log Connection_ConnectionShutDown

        if (disposed) return;

        TryConnect();
    }

    private void Connection_CallbackException(object sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
    {
        // Log Connection_CallbackException
        
        if (disposed) return;

        TryConnect();
    }

    private void Connection_ConnectionBlocked(object sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
    {
        // Log Connection_ConnectionBlocked
        
        if (disposed) return;

        TryConnect();
    }
}