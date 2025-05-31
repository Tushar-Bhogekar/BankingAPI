using BankingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet("account-details")]
        public async Task<IActionResult> GetAccountDetails()
        {
            // Extract userId from the JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Fetch account details using the repository
            var account = await _accountRepository.GetAccountDetailsAsync(userId);
            if (account == null)
            {
                return NotFound("Account not found.");
            }

            // Prepare account details to be returned
            var accountDetails = new
            {
                account.AccountNumber,
                Name = $"{account.User.FirstName} {account.User.LastName}",
                AccountType = "Saving",
                Balance = account.Balance,
                CreatedAt = account.User.CreatedAt,
                Status = "Active",
                BranchName = "Mumbai Main",  // You can fetch from database if needed
                BranchCode = "MM001",       // You can fetch from database if needed
                Currency = "INR",           // Assuming INR
                Message = $"Welcome, {account.User.FirstName}! Explore our mobile banking app for faster access."
            };

            return Ok(accountDetails);
        }
    }
}
