using EventBus.Base.Events;

namespace EventBus.UnitTest.Events;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public int Id { get; set; }

    public OrderCreatedIntegrationEvent(int id)
    {
        Id = id;
    }
}
