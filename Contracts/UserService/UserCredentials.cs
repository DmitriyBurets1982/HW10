namespace Contracts.UserService;

public sealed class UserCredentials(string username, string password)
{
    public string Username { get; } = username;

    public string Password { get; } = password;
}
