using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingContext _context;

        public TransactionRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.Include(u => u.Beneficiaries).FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByAccountNumberAsync(string accountNumber)
        {
            return await _context.Users.Include(u => u.Account).FirstOrDefaultAsync(u => u.Account.AccountNumber == accountNumber);
        }

        public async Task<Beneficiary?> GetBeneficiaryByAccountNumberAsync(string accountNumber)
        {
            return await _context.Beneficiaries.FirstOrDefaultAsync(b => b.AccountNumber == accountNumber);
        }

        public async Task<bool> AddBeneficiaryAsync(User user, Beneficiary newBeneficiary)
        {
            user.Beneficiaries.Add(newBeneficiary);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Transaction?> PerformFundTransferAsync(User fromUser, Beneficiary toBeneficiary, FundTransferDto transferDto)
        {
            if (fromUser.Account.Balance < transferDto.Amount)
                return null;

            // Deduct balance
            fromUser.Account.Balance -= transferDto.Amount;

            // Create transaction log
            var transaction = new Transaction
            {
                FromAccount = transferDto.FromAccount,
                Amount = transferDto.Amount,
                Mode = transferDto.Mode,
                Remark = transferDto.Remark,
                TransactionDate = DateTime.UtcNow,
                BeneficiaryId = toBeneficiary.Id
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<Transaction>> GetTransactionsByAccountNumberAsync(string accountNumber)
        {
            return await _context.Transactions
                .Include(t => t.Beneficiary) // Include Beneficiary data
                .Where(t => t.FromAccount == accountNumber)
                .ToListAsync();
        }

        public async Task<List<Beneficiary>> GetBeneficiariesByUserIdAsync(string userId)
        {
            return await _context.Beneficiaries
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

    }
}