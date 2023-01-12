using OrderService.Domain.SeedWork;

namespace OrderService.Domain.AggregateModels.BuyerAggregate;

public class CardType : Enumeration
{
    public static CardType AmericanExpress = new(1, nameof(AmericanExpress));
    public static CardType Visa = new(2, nameof(Visa));
    public static CardType MasterCard = new(3, nameof(MasterCard));

    public CardType(int id, string name)
        : base(id, name)
    {

    }
}