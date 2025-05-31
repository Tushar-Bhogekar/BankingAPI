
using BankingAPI.Models;
using BankingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpPost("add-beneficiary")]
        [Authorize]
        public async Task<IActionResult> AddBeneficiary([FromBody] BeneficiaryDto beneficiaryDto)
        {
            if (beneficiaryDto == null || beneficiaryDto.AccountNumber != beneficiaryDto.ReEnterAccountNumber)
                return BadRequest("Account numbers do not match.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token or user not authenticated.");

            var user = await _transactionRepository.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var newBeneficiary = new Beneficiary
            {
                Name = beneficiaryDto.Name,
                AccountNumber = beneficiaryDto.AccountNumber,
                Nickname = beneficiaryDto.Nickname
            };

            var result = await _transactionRepository.AddBeneficiaryAsync(user, newBeneficiary);
            if (!result)
                return StatusCode(500, "Error while adding beneficiary.");

            return Ok(new { message = "Beneficiary added successfully." });
        }

        [HttpPost("fund-transfer")]
        [Authorize]
        public async Task<IActionResult> FundTransfer([FromBody] FundTransferDto transferDto)
        {
            if (transferDto == null)
                return BadRequest("Invalid transfer data.");

            var fromUser = await _transactionRepository.GetUserByAccountNumberAsync(transferDto.FromAccount);
            if (fromUser == null || fromUser.Account == null)
                return NotFound("Sender account not found.");

            if (fromUser.TransactionPassword != transferDto.TransactionPassword)
                return BadRequest("Invalid transaction password.");

            var toBeneficiary = await _transactionRepository.GetBeneficiaryByAccountNumberAsync(transferDto.ToAccount);
            if (toBeneficiary == null)
                return NotFound("Beneficiary account not found.");

            var validModes = new[] { "NEFT", "RTGS", "IMPS" };
            if (!validModes.Contains(transferDto.Mode.ToUpper()))
                return BadRequest("Invalid transfer mode.");

            var transaction = await _transactionRepository.PerformFundTransferAsync(fromUser, toBeneficiary, transferDto);
            if (transaction == null)
                return BadRequest("Insufficient balance for the transfer.");

            return Ok(new
            {
                Message = "Transfer Successful",
                Amount = transaction.Amount,
                ReferenceId = transaction.ReferenceId,
                Mode = transaction.Mode,
                PaidToBeneficiary = toBeneficiary.Name,
                FromAccount = transaction.FromAccount,
                Date = transaction.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Remark = transaction.Remark
            });
        }

        [HttpGet("view-transactions")]
        [Authorize]
        public async Task<IActionResult> ViewTransactions()
        {
            // Get the logged-in user's Account Number from the JWT token
            var accountNumber = User.FindFirst("AccountNumber")?.Value;
            if (string.IsNullOrEmpty(accountNumber))
                return Unauthorized("Invalid token or user not authenticated.");

            // Get the user by their account number
            var user = await _transactionRepository.GetUserByAccountNumberAsync(accountNumber);
            if (user == null)
                return NotFound("User not found.");

            // Fetch all transactions associated with the user's account
            var transactions = await _transactionRepository.GetTransactionsByAccountNumberAsync(accountNumber);
            if (transactions == null || !transactions.Any())
                return NotFound("No transactions found for this account.");

            // Filter out transactions without a valid beneficiary
            var filteredTransactions = transactions
                .Where(t => t.Beneficiary != null && !string.IsNullOrEmpty(t.Beneficiary.Name))
                .Select(t => new
                {
                    BeneficiaryAccount = t.Beneficiary.AccountNumber,
                    BeneficiaryName = t.Beneficiary.Name,
                    t.Amount,
                    t.TransactionDate,
                    t.Mode,
                    t.Remark,
                    t.ReferenceId
                });

            if (!filteredTransactions.Any())
                return NotFound("No valid transactions found with beneficiaries.");

            // Return the filtered transactions to the user
            return Ok(new
            {
                Message = "Transactions retrieved successfully.",
                Transactions = filteredTransactions
            });
        }


        [HttpGet("view-beneficiaries")]
        [Authorize]
        public async Task<IActionResult> ViewBeneficiaries()
        {
            // Retrieve the User ID from the JWT Token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token or user not authenticated.");

            // Fetch user details using the User ID
            var user = await _transactionRepository.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Retrieve the list of beneficiaries for this user
            var beneficiaries = await _transactionRepository.GetBeneficiariesByUserIdAsync(userId);
            if (beneficiaries == null || !beneficiaries.Any())
                return NotFound("No beneficiaries found for this user.");

            // Return the list of beneficiaries
            return Ok(new
            {
                Message = "Beneficiaries retrieved successfully.",
                Beneficiaries = beneficiaries.Select(b => new
                {
                    b.Name,
                    b.AccountNumber,
                    b.Nickname
                })
            });
        }

    }
}


