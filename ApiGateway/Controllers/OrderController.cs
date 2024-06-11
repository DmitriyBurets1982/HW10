using System.Text;
using System.Text.Json;
using Contracts.OrderService;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(HttpClient httpClient, IConfiguration configuration, ILogger<OrderController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var url = $"{configuration["OrderService"]}/orderservice/order";
        logger.LogInformation("OrderService url: '{Url}'", url);

        await httpClient.PostAsync(url, httpContent);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int userId)
    {
        var url = $"{configuration["OrderService"]}/orderservice/order?userId={userId}";
        logger.LogInformation("OrderService url: '{Url}'", url);

        using var response = await httpClient.GetAsync(url);
        var orders = await response.Content.ReadFromJsonAsync<IEnumerable<Order>>();
        
        return Ok(orders);
    }
}
