using Contracts.OrderService;

namespace OrderService.Services;

public interface IOrderService
{
    Task CreateOrder(Order order);

    IList<Order> GetOrders(int accountId);
}
