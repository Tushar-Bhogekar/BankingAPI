using BankingAPI.Models;

namespace BankingAPI.Repositories
{
    public interface ITransactionRepository
    {
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByAccountNumberAsync(string accountNumber);
        Task<Beneficiary?> GetBeneficiaryByAccountNumberAsync(string accountNumber);
        Task<bool> AddBeneficiaryAsync(User user, Beneficiary newBeneficiary);
        Task<Transaction?> PerformFundTransferAsync(User fromUser, Beneficiary toBeneficiary, FundTransferDto transferDto);

        Task<List<Transaction>> GetTransactionsByAccountNumberAsync(string accountNumber);
        Task<List<Beneficiary>> GetBeneficiariesByUserIdAsync(string userId);
    }
}
