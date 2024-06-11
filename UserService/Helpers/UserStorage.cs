using System.Collections.Concurrent;
using Contracts.UserService;

namespace UserService.Helpers;

public static class UserStorage
{
    private static readonly ConcurrentDictionary<int, User> _users = new();

    public static User? GetValueOrDefault(int id)
    {
        return _users.GetValueOrDefault(id);
    }

    public static User? GetByUserName(string userName)
    {
        return _users.Values.FirstOrDefault(x => x.UserName == userName);
    }

    public static User Register(int accountId, string firstName, string lastName, string userName)
    {
        var userId = _users.Count + 1;
        var user = new User { Id = userId, AccountId = accountId, FirstName = firstName, LastName = lastName, UserName = userName };
        _users.TryAdd(userId, user);

        return user;
    }
}
