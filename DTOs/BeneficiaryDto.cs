using System.ComponentModel.DataAnnotations;
namespace BankingAPI.Models
{
    public class BeneficiaryDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Account Number must be between 10 and 20 characters.")]
        public required string AccountNumber { get; set; }

        [Required(ErrorMessage = "Please re-enter the account number.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Account Number must be between 10 and 20 characters.")]
        public required string ReEnterAccountNumber { get; set; }

        [StringLength(50, ErrorMessage = "Nickname cannot exceed 50 characters.")]
        public string? Nickname { get; set; }
    }


    public class FundTransferDto
    {
        [Required(ErrorMessage = "From Account is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "From Account must be between 10 and 20 characters.")]
        public required string FromAccount { get; set; }

        [Required(ErrorMessage = "To Account is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "To Account must be between 10 and 20 characters.")]
        public required string ToAccount { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Transaction Mode is required.")]
        [StringLength(5, ErrorMessage = "Mode should be 4 or 5 characters long (e.g., NEFT, RTGS, IMPS).")]
        public required string Mode { get; set; } // NEFT, RTGS, IMPS

        [Required(ErrorMessage = "Transaction Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Transaction Password should be between 6 and 20 characters.")]
        public required string TransactionPassword { get; set; }

        [StringLength(100, ErrorMessage = "Remark cannot exceed 100 characters.")]
        public string? Remark { get; set; }
    }

}
