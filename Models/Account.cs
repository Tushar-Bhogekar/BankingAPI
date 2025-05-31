using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models
{
    public class Account
    {
        [Key]
        public required string AccountNumber { get; set; } // Account number as primary key

        [Required(ErrorMessage = "Balance is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a non-negative number.")]
        public decimal Balance { get; set; } = 1000; // Default balance

        [Required(ErrorMessage = "Account type is required.")]
        [StringLength(50, ErrorMessage = "Account type can't be longer than 50 characters.")]
        public string AccountType { get; set; } = "Saving"; // Default account type

        [Required(ErrorMessage = "UserId is required.")]
        [StringLength(50)]
        public required string UserId { get; set; } // Foreign key

        // Navigation property for the user
        public User? User { get; set; }
    }
}
