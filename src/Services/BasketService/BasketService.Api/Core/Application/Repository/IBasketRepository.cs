using BasketService.Api.Core.Domain.Models;

namespace BasketService.Api.Core.Application.Repository;

public interface IBasketRepository
{
    Task<CustomerBasket> GetBasketAsync(string customerId);

    IEnumerable<string> GetUsers();

    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);

    Task<bool> DeleteBasketAsync(string id);
}