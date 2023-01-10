using BasketService.Api.Core.Application.Repository;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace BasketService.Api.IntegrationEvents.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<OrderCreatedIntegrationEvent> _logger;

    public OrderCreatedIntegrationEventHandler(IBasketRepository basketRepository, ILogger<OrderCreatedIntegrationEvent> logger)
    {
        _basketRepository = basketRepository;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        _logger.LogInformation("----- Handling Integration Event: {IntegrationEventId} at BasketService.Api - ({@IntegrationEvent})", @event.Id, @event);

        await _basketRepository.DeleteBasketAsync(@event.UserId.ToString());
    }
}