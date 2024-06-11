using System.Text.Json;
using System.Text;
using AutoMapper;
using MassTransit;
using System.Collections.Concurrent;
using Contracts.AccountService;
using Contracts.NotificationService;
using Contracts.OrderService;

namespace OrderService.Services;

public class OrderService(HttpClient httpClient, IMapper mapper, IBus bus, IConfiguration configuration, ILogger<OrderService> logger) : IOrderService
{
    private static readonly ConcurrentBag<Order> _orders = [];

    public async Task CreateOrder(Order order)
    {
        var accountOperation = mapper.Map<AccountOperationDto>(order);
        var httpContent = new StringContent(JsonSerializer.Serialize(accountOperation), Encoding.UTF8, "application/json");
        var url = $"{configuration["AccountService"]}/accountservice/account/withdraw";
        logger.LogInformation("AccountService url: '{Url}'", url);

        using var response = await httpClient.PostAsync(url, httpContent);
        var isCreated = await response.Content.ReadFromJsonAsync<bool>();

        var notification = new Notification
        {
            AccountId = order.AccountId,
            Price = order.Price,
            Result = isCreated ? NotificationResult.Accepted : NotificationResult.Rejected,
        };

        await bus.Publish(notification);

        if (isCreated)
        {
            order.OrderId = _orders.Count + 1;
            _orders.Add(order);
        }
    }

    public IList<Order> GetOrders(int accountId)
    {
        return _orders.Where(x => x.AccountId == accountId).ToList();
    }
}

