namespace Contracts.UserService;

public class User
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string UserName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
