using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly BankingContext _context;

        public AuthRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUserIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUserByAccountNumberAsync(string accountNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountNumber == accountNumber);
        }

        public async Task AddOtpAsync(Otp otp)
        {
            _context.Otps.Add(otp);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Otp> GetOtpByOtpAsync(string otp)
        {
            return await _context.Otps
                .Where(o => o.OTP == otp)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveOtpAsync(Otp otp)
        {
            _context.Otps.Remove(otp);
            await SaveChangesAsync();
        }

        public async Task<Otp> GenerateOtpForPasswordResetAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return null;
            }

            string otp = new Random().Next(100000, 999999).ToString();

            var otpRecord = new Otp
            {
                AccountNumber = user.AccountNumber,
                OTP = otp,
                CreatedAt = DateTime.Now
            };

            _context.Otps.Add(otpRecord);
            await _context.SaveChangesAsync();

            return otpRecord;
        }

        public async Task<Otp> GetOtpByOtpAndAccountNumberAsync(string otp, string accountNumber)
        {
            return await _context.Otps
                .FirstOrDefaultAsync(o => o.OTP == otp && o.AccountNumber == accountNumber);
        }


    }
}

