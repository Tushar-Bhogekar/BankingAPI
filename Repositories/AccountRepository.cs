using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingContext _context;

        public AccountRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<Account> GetAccountDetailsAsync(string userId)
        {
            var account = await _context.Accounts
                                         .Include(a => a.User) // Ensure related user details are loaded
                                         .FirstOrDefaultAsync(a => a.UserId == userId);

            return account;
        }
    }
}
