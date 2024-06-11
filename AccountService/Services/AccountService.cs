using System.Collections.Concurrent;
using Contracts.AccountService;

namespace AccountService.Services;

public class AccountService : IAccountService
{
    private readonly ConcurrentDictionary<int, Account> _accounts = new();

    public Account CreateAccount(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) 
        {
            throw new ArgumentNullException(nameof(userName));
        }

        if (_accounts.Values.Any(x => x.UserName == userName))
        {
            throw new InvalidOperationException($"User '{userName}' has been already registered");
        }

        var account = new Account
        {
            Id = _accounts.Count + 1,
            UserName = userName,
        };

        _accounts.TryAdd(account.Id, account);
        return account;
    }

    public Account? GetAccount(int id)
    {
        if (_accounts.TryGetValue(id, out var account))
        {
            return account;
        }

        return null;
    }

    public void IncreaseBalance(Account account, double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

        account.Balance += value;
    }

    public bool DecreaseBalance(Account account, double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

        if (account.Balance >= value) 
        {
            account.Balance -= value;
            return true;
        }

        return false;
    }
}
