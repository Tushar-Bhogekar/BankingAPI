
using BankingAPI.Data;
using BankingAPI.Models;
using System.Linq;

namespace BankingAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BankingContext _context;

        public UserRepository(BankingContext context)
        {
            _context = context;
        }

        public User GetUserById(string userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
