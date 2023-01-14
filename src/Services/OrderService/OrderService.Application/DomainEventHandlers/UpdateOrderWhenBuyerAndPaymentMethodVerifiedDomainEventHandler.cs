using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Events;

namespace OrderService.Application.DomainEventHandlers;

public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent buyerAndPaymentMethodVerifiedEvent, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository
            .GetByIdAsync(buyerAndPaymentMethodVerifiedEvent.OrderId);

        orderToUpdate.SetBuyerId(buyerAndPaymentMethodVerifiedEvent.Buyer.Id);
        orderToUpdate.SetPaymentMethodId(buyerAndPaymentMethodVerifiedEvent.Payment.Id);

        // set methods to validate
    }
}