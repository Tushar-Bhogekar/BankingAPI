using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BankingContext _context;

        public UserController(BankingContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewAccount([FromBody] UserRegistrationDto registrationDto)
        {
            if (registrationDto == null)
                return BadRequest("Invalid data.");

            // Validate required fields
            if (string.IsNullOrEmpty(registrationDto.FirstName) ||
                string.IsNullOrEmpty(registrationDto.LastName) ||
                string.IsNullOrEmpty(registrationDto.MobileNumber) ||
                string.IsNullOrEmpty(registrationDto.Email) ||
                string.IsNullOrEmpty(registrationDto.AadharCardNumber) ||
                registrationDto.DateOfBirth == default)
            {
                return BadRequest("Required fields are missing.");
            }

            // Check if the email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registrationDto.Email);
            if (existingUser != null)
                return BadRequest("User with this email already exists.");

            // Generate the unique UserId
            string generatedUserId = GenerateUniqueUserId(registrationDto.FirstName, registrationDto.DateOfBirth.Year);
            var accountNo = GenerateUniqueAccountNumber();

            // Create a new user with a generated account number and UserId
            var newUser = new User
            {
                UserId = generatedUserId,
                FirstName = registrationDto.FirstName,
                MiddleName = registrationDto.MiddleName,
                LastName = registrationDto.LastName,
                MobileNumber = registrationDto.MobileNumber,
                Email = registrationDto.Email,
                AadharCardNumber = registrationDto.AadharCardNumber,
                DateOfBirth = registrationDto.DateOfBirth,
                Address = registrationDto.Address,
                City = registrationDto.City,
                State = registrationDto.State,
                OccupationType = registrationDto.OccupationType,
                SourceOfIncome = registrationDto.SourceOfIncome,
                AccountNumber = accountNo,
                CreatedAt = DateTime.UtcNow,
                Password = registrationDto.Password,
                LoginAttempts = 0,
                IsLocked = false,
                AccountStatus = "Pending" // Set initial status to Pending
            };

            var newAccount = new Account
            {
                AccountNumber = accountNo,
                Balance = 1000, // Default balance
                UserId = generatedUserId // Link the account to the user
            };

            _context.Users.Add(newUser);
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful. Your account will be approved by the admin. Details will be sent to your registered email upon approval."
            });
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                // Retrieve all users from the database
                var users = _context.Users.ToList();

                if (!users.Any())
                {
                    return NotFound("No users found.");
                }

                // Return the list of users
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Handle any errors and return a 500 Internal Server Error
                return StatusCode(500, new { Message = "An error occurred while retrieving users.", Error = ex.Message });
            }
        }

        // Admin endpoint to approve accounts and send email
        [HttpPut("UpdateAccountStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAccountStatus([FromBody] AccountStatusUpdateDto statusUpdateDto)
        {
            if (statusUpdateDto == null)
                return BadRequest("Invalid data.");

            var validStatuses = new[] { "Pending", "Approved", "Disapproved" };
            if (!validStatuses.Contains(statusUpdateDto.AccountStatus))
                return BadRequest($"Invalid account status. Valid statuses are: {string.Join(", ", validStatuses)}.");

            // Find the user by UserId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == statusUpdateDto.UserId);

            if (user == null)
                return NotFound("User not found.");

            // Update the AccountStatus
            user.AccountStatus = statusUpdateDto.AccountStatus;

            try
            {
                // Save changes to the database
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                if (statusUpdateDto.AccountStatus == "Approved")
                {
                    // Send the account approval email
                    SendAccountApprovalEmail(user.Email, user.AccountNumber, user.UserId);
                }

                return Ok(new { message = "Account status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating account status: " + ex.Message);
            }
        }

        // Method to generate a unique UserId
        private string GenerateUniqueUserId(string firstName, int birthYear)
        {
            string userId = firstName.Substring(0, 3).ToUpper() + birthYear;

            // Ensure the UserId is unique by checking the database
            while (_context.Users.Any(u => u.UserId == userId))
            {
                birthYear++; // Increment the year to create a unique UserId
                userId = firstName.Substring(0, 3).ToUpper() + birthYear;
            }

            return userId;
        }

        // Method to generate unique account numbers
        private string GenerateUniqueAccountNumber()
        {
            string newAccountNumber;
            bool isUnique;

            do
            {
                var random = new Random();
                newAccountNumber = random.Next(100000, 999999).ToString() +
                                  random.Next(100000, 999999).ToString();
                isUnique = !_context.Users.Any(u => u.AccountNumber == newAccountNumber);
            } while (!isUnique);

            return newAccountNumber;
        }

        // Method to send account approval email
        private void SendAccountApprovalEmail(string recipientEmail, string accountNumber, string userId)
        {
            try
            {
                // SMTP details
                string smtpServer = "smtp.gmail.com";
                int port = 587;
                string senderEmail = "tusharbho58@gmail.com";
                string password = "jdipqqpskeceozha";

                // Set up the SMTP client
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(senderEmail, password),
                    EnableSsl = true,
                };

                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Your Account Approval Details",
                    Body = $"Dear User,\n\nYour account has been successfully approved.\n\nAccount Type: Saving\nAccount Number: {accountNumber}\nUser ID: {userId}\n\nThank you for choosing our services!",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(recipientEmail);

                // Send the email
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }

        [HttpPut("update")]

        public async Task<IActionResult> UpdateUserDetails([FromBody] UserUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest("Invalid data.");

            // Find the user by UserId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updateDto.UserId);

            if (user == null)
                return NotFound("User not found.");

            // Update necessary fields
            user.FirstName = updateDto.FirstName ?? user.FirstName;
            user.MiddleName = updateDto.MiddleName ?? user.MiddleName;
            user.LastName = updateDto.LastName ?? user.LastName;
            user.MobileNumber = updateDto.MobileNumber ?? user.MobileNumber;
            user.Email = updateDto.Email ?? user.Email;
            user.Address = updateDto.Address ?? user.Address;
            user.City = updateDto.City ?? user.City;
            user.State = updateDto.State ?? user.State;
            user.OccupationType = updateDto.OccupationType ?? user.OccupationType;
            user.SourceOfIncome = updateDto.SourceOfIncome ?? user.SourceOfIncome;

            try
            {
                // Save changes to the database
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "User details updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating user details: " + ex.Message);
            }
        }


        [HttpGet("details")]
        [Authorize] // Ensure only authenticated users can access this endpoint
        public IActionResult GetUserDetails()
        {
            // Retrieve the logged-in user's ID from the JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the userId is null or empty (not authenticated)
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Query the database to get user details
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            // If user not found
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Return user details in the response
            return Ok(user);
        }

    }
}


