using System.Text.Json;
using AutoMapper;
using Contracts.OrderService;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("orderservice/[controller]")]
public class OrderController(IOrderService orderService, IMapper mapper, ILogger<OrderController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Dto}' was called", nameof(CreateOrder), JsonSerializer.Serialize(dto));

        try
        {
            var newOrder = mapper.Map<Order>(dto);
            await orderService.CreateOrder(newOrder);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error while executing '{nameof(CreateOrder)}' method");
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public IActionResult GetOrders(int accountId)
    {
        logger.LogInformation("'{MethodName}' with parameter '{AccountId}' was called", nameof(GetOrders), accountId);

        return Ok(orderService.GetOrders(accountId));
    }
}
