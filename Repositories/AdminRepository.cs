using BankingAPI.Data;
using BankingAPI.Models;

namespace BankingAPI.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly BankingContext _context;

        public AdminRepository(BankingContext context)
        {
            _context = context;
        }

        public Admin? ValidateAdmin(string username, string password)
        {
            return _context.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);
        }
    }
}