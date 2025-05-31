using BankingAPI.Repositories;
using BankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternetBankingController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public InternetBankingController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpPost("register-internet-banking")]
        public IActionResult RegisterInternetBanking([FromBody] InternetBankingRegistrationDto registrationDto)
        {
            if (registrationDto == null)
                return BadRequest("Invalid data.");

            var userId = User.Identity.Name; // Get the UserId from claims
            var user = _userRepository.GetUserById(userId);
            if (user == null)
                return Unauthorized("User not found.");

            // Validate passwords
            if (string.IsNullOrEmpty(registrationDto.Password) || string.IsNullOrEmpty(registrationDto.TransactionPassword))
                return BadRequest("Passwords are required.");

            if (registrationDto.Password != registrationDto.ConfirmPassword)
                return BadRequest("Password and Confirm Password do not match.");

            if (registrationDto.Password != user.Password)
                return BadRequest("Password must match the existing login password.");

            if (registrationDto.TransactionPassword != registrationDto.ConfirmTransactionPassword)
                return BadRequest("Transaction Password and Confirm Transaction Password do not match.");

            // Update the user with the transaction password
            user.TransactionPassword = registrationDto.TransactionPassword;

            _userRepository.UpdateUser(user);
            _userRepository.SaveChanges();

            return Ok(new { message = "Internet Banking registration successful." });
        }
    }
}
