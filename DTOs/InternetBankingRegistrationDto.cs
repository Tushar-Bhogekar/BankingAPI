using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models
{
    public class InternetBankingRegistrationDto
    {

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Confirm Password must be between 6 and 20 characters.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Transaction Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Transaction Password must be between 6 and 20 characters.")]
        public required string TransactionPassword { get; set; }

        [Required(ErrorMessage = "Confirm Transaction Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Confirm Transaction Password must be between 6 and 20 characters.")]
        public required string ConfirmTransactionPassword { get; set; }
    }
}