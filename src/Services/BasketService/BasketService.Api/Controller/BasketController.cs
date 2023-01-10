using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketService.Api.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IBasketRepository basketRepository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
    {
        _basketRepository = basketRepository;
        _identityService = identityService;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpHead]
    public IActionResult Get()
    {
        return Ok("Basket Service is up and running!");
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await _basketRepository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket customerBasket)
    {
        return Ok(await _basketRepository.UpdateBasketAsync(customerBasket));
    }

    [HttpPost]
    [Route("additem")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem basketItem)
    {
        var userId = _identityService.GetUserName().ToString();

        var basket = await _basketRepository.GetBasketAsync(userId);

        if(basket == null)
        {
            basket = new CustomerBasket(userId);
        }

        basket.Items.Add(basketItem);

        await _basketRepository.UpdateBasketAsync(basket);

        return Ok();
    }

    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
    {
        var userId = basketCheckout.Buyer;

        var basket = await _basketRepository.GetBasketAsync(userId);

        if(basket == null)
        {
            return BadRequest();
        }

        var userName = _identityService.GetUserName();

        var eventMessage = new OrderCreatedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
            basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
            basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basket);

        try
        {
            // Listen itself to clean the basket
            // It is listened by OrderApi to start the process
            _eventBus.Publish(eventMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {BaskerService.App}", eventMessage.Id);
            
            throw;
        }

        return Accepted();
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketByIdAsync(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
    }
}