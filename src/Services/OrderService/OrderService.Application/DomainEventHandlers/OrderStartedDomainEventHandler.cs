using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;

namespace OrderService.Application.DomainEventHandlers;

public class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
{
    private readonly IBuyerRepository _buyerRepository;

    public OrderStartedDomainEventHandler(IBuyerRepository buyerRepository)
    {
        _buyerRepository = buyerRepository;
    }

    public async Task Handle(OrderStartedDomainEvent orderStartedDomainEvent, CancellationToken cancellationToken)
    {
        var cardTypeId = (orderStartedDomainEvent.CardTypeId != 0) ? orderStartedDomainEvent.CardTypeId : 1;

        var buyer = await _buyerRepository.GetSingleAsync(i => i.Name == orderStartedDomainEvent.UserName,
            i => i.PaymentMethods);

        bool buyerOriginallyExisted = buyer != null;

        if (!buyerOriginallyExisted)
        {
            buyer = new Buyer(orderStartedDomainEvent.UserName);
        }

        buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                       $"Payment Method on {DateTime.UtcNow}",
                                       orderStartedDomainEvent.CardNumber,
                                       orderStartedDomainEvent.CardSecurityNumber,
                                       orderStartedDomainEvent.CardHolderName,
                                       orderStartedDomainEvent.CardExpiration,
                                       orderStartedDomainEvent.Order.Id);

        var buyerUpdated = buyerOriginallyExisted ?
            _buyerRepository.Update(buyer) :
            await _buyerRepository.AddAsync(buyer);

        await _buyerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        // order status changed event may be fired here
    }
}