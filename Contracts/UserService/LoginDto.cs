namespace Contracts.UserService;

public class LoginDto
{
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
}
