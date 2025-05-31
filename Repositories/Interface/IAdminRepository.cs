using BankingAPI.Models;

namespace BankingAPI.Repositories
{
    public interface IAdminRepository
    {
        Admin? ValidateAdmin(string username, string password);
    }
}
