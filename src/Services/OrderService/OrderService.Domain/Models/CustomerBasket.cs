namespace OrderService.Domain.Models;

public class CustomerBasket
{
    public string BuyerId { get; set; }

    public List<BasketItem> Items { get; set; } = new List<BasketItem>();

    public CustomerBasket(string customerId)
    {
        BuyerId = customerId;
        Items = new List<BasketItem>();
    }
}