using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers;

public class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
{
    private readonly ILogger<OrderPaymentSuccessIntegrationEventHandler> _logger;

    public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
    {
        // Send Fail Notification (Sms, Mail, Push)

        _logger.LogInformation($"Order Payment has been completed successfully with OrderId: {@event.OrderId}");

        return Task.CompletedTask;
    }
}