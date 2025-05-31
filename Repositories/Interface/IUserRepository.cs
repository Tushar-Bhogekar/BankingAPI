using BankingAPI.Models;

namespace BankingAPI.Repositories
{
    public interface IUserRepository
    {
        User GetUserById(string userId);
        void UpdateUser(User user);
        void SaveChanges();
    }
}
