namespace Contracts.OrderService;

public class Order
{
    public int OrderId { get; set; }
    public int AccountId { get; set; }
    public double Price { get; set; }
}
