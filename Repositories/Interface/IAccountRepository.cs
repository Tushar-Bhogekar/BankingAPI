using BankingAPI.Models;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountDetailsAsync(string userId);
    }
}
