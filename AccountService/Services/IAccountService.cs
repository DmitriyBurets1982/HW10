using Contracts.AccountService;

namespace AccountService.Services
{
    public interface IAccountService
    {
        Account CreateAccount(string userName);

        Account? GetAccount(int id);

        void IncreaseBalance(Account account, double value);

        bool DecreaseBalance(Account account, double value);
    }
}
