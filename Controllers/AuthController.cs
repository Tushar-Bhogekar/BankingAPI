

using BankingAPI.Models;
using BankingAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MailKit.Net.Smtp;


namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _authRepository.GetUserByUserIdAsync(loginRequest.UserId);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (user.IsLocked)
                return Unauthorized("Your account is locked. Please reset your password to unlock.");

            if (user.AccountStatus != "Approved")
                return Unauthorized($"Your account status is '{user.AccountStatus}'. Please contact admin.");

            if (user.Password != loginRequest.Password)
            {
                user.LoginAttempts++;

                if (user.LoginAttempts >= 3)
                {
                    user.IsLocked = true;
                }

                await _authRepository.SaveChangesAsync();
                return Unauthorized("Invalid credentials.");
            }

            // Reset login attempts on successful login
            user.LoginAttempts = 0;
            await _authRepository.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Message = "Login successful!",
                Token = token // Return the JWT token to the client
            });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserId),
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Role, "User"), // Add the user's role if needed
                new Claim("AccountNumber", user.AccountNumber)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("forgot-userid")]
        public async Task<IActionResult> ForgotUserId([FromBody] AccountNumberDto accountNumberDto)
        {
            string accountNumber = accountNumberDto.AccountNumber;

            var user = await _authRepository.GetUserByAccountNumberAsync(accountNumber);
            if (user == null) return NotFound("Account not found.");

            string otp = new Random().Next(100000, 999999).ToString();

            var otpRecord = new Otp
            {
                AccountNumber = accountNumber,
                OTP = otp,
                CreatedAt = DateTime.Now
            };
            await _authRepository.AddOtpAsync(otpRecord);

            Console.WriteLine($"Generated OTP: {otp} for Account: {accountNumber} at {DateTime.Now}");

            SendEmail(user.Email, "Your OTP for User ID Retrieval", $"Your OTP for User ID retrieval is: {otp}");

            return Ok(new { message = "OTP sent to your registered email address." });
        }

        [HttpPost("verify-otp-for-userid")]
        public async Task<IActionResult> VerifyOtpForUserId([FromBody] VerifyOtpRequest request)
        {
            // Log for debugging
            Console.WriteLine($"Verifying OTP: {request.Otp} at {DateTime.Now}");

            if (string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest(new { message = "OTP is required." });
            }

            var otpRecord = await _authRepository.GetOtpByOtpAsync(request.Otp);

            if (otpRecord == null)
                return BadRequest(new { message = "OTP session expired." });

            // Check if the OTP has expired (e.g., 5 minutes expiration time)
            if (DateTime.Now.Subtract(otpRecord.CreatedAt).TotalMinutes > 5)
            {
                await _authRepository.RemoveOtpAsync(otpRecord);
                return BadRequest(new { message = "OTP session expired." });
            }

            // OTP is valid, proceed with user ID retrieval
            var user = await _authRepository.GetUserByAccountNumberAsync(otpRecord.AccountNumber);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Send User ID via email
            SendEmail(user.Email, "Your User ID", $"Your User ID is: {user.UserId}");

            // Remove OTP from the database after successful verification
            await _authRepository.RemoveOtpAsync(otpRecord);

            return Ok(new { message = "User ID has been sent to your email." });
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            // Log for debugging
            Console.WriteLine($"Processing password reset request for UserId: {request.UserId} at {DateTime.Now}");

            // Validate userId
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { message = "UserId is required." });
            }

            var otpRecord = await _authRepository.GenerateOtpForPasswordResetAsync(request.UserId);
            if (otpRecord == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Send OTP via email
            var user = await _authRepository.GetUserByUserIdAsync(request.UserId);
            SendEmail(user.Email, "Your OTP for Password Reset", $"Your OTP for resetting your password is: {otpRecord.OTP}");

            return Ok(new { message = "OTP sent to your registered email address." });
        }

        [HttpPost("verify-otp-for-password-reset")]
        public async Task<IActionResult> VerifyOtpForPasswordReset([FromBody] VerifyOtpForPasswordResetRequest request)
        {
            // Log received OTP and userId for debugging
            Console.WriteLine($"OTP received: {request.Otp}, User ID: {request.UserId}");

            // Look for the user based on userId using repository method
            var user = await _authRepository.GetUserByUserIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Look for OTP that matches the AccountNumber (from user) and the provided OTP using repository method
            var otpRecord = await _authRepository.GetOtpByOtpAndAccountNumberAsync(request.Otp, user.AccountNumber);

            if (otpRecord == null)
            {
                return BadRequest(new { message = "Invalid OTP." });
            }

            // Check if OTP has expired (e.g., 5 minutes expiration time)
            if (DateTime.Now.Subtract(otpRecord.CreatedAt).TotalMinutes > 5)
            {
                await _authRepository.RemoveOtpAsync(otpRecord);  // Remove expired OTP from database
                return BadRequest(new { message = "OTP session expired." });
            }

            // Mark the OTP as verified
            otpRecord.IsVerified = true;
            await _authRepository.SaveChangesAsync();

            // OTP is valid
            return Ok(new { message = "OTP verified. Please proceed to reset your password." });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // Log received data for debugging
            Console.WriteLine($"New Password: {request.NewPassword}, Confirm Password: {request.ConfirmPassword}");

            // Check if OTP is provided
            if (string.IsNullOrEmpty(request.Otp))
                return BadRequest(new { message = "OTP is required." });

            // Validate OTP
            var otpRecord = await _authRepository.GetOtpByOtpAsync(request.Otp);

            if (otpRecord == null)
                return BadRequest(new { message = "Invalid OTP." });

            if (!otpRecord.IsVerified)
                return BadRequest(new { message = "OTP is not verified. Please verify OTP before resetting the password." });

            var user = await _authRepository.GetUserByAccountNumberAsync(otpRecord.AccountNumber);

            if (user == null)
                return NotFound(new { message = "User not found." });

            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match." });

            // Reset password and unlock account
            user.Password = request.NewPassword;
            user.IsLocked = false; // Unlock account after password reset
            user.LoginAttempts = 0; // Reset login attempts

            // Remove OTP from the database after successful password reset
            await _authRepository.RemoveOtpAsync(otpRecord);

            // Save changes in the database
            await _authRepository.SaveChangesAsync();

            return Ok(new { message = "Password reset successfully!" });
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("BankingAPI", "tusharbho58@gmail.com"));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            email.Body = new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, false);
            smtp.Authenticate("tusharbho58@gmail.com", "jdipqqpskeceozha");
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}

