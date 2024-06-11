namespace Contracts.AccountService;

public class Account
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public double Balance { get; set; }
}
