using BankingAPI.Models;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public interface IAuthRepository
    {
        Task<User> GetUserByUserIdAsync(string userId);
        Task<User> GetUserByAccountNumberAsync(string accountNumber);
        Task AddOtpAsync(Otp otp);
        Task SaveChangesAsync();
        Task<Otp> GetOtpByOtpAsync(string otp);
        Task RemoveOtpAsync(Otp otp);
        Task<Otp> GenerateOtpForPasswordResetAsync(string userId);

        Task<Otp> GetOtpByOtpAndAccountNumberAsync(string otp, string accountNumber);

    }
}