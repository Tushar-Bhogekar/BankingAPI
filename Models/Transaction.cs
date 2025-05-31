using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "From Account is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "From Account must be between 10 and 20 characters.")]
        public required string FromAccount { get; set; }



        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Transaction Mode is required.")]
        [StringLength(5, ErrorMessage = "Mode should be 4 or 5 characters long (e.g., NEFT, RTGS, IMPS).")]
        public required string Mode { get; set; } // NEFT, RTGS, IMPS

        [StringLength(100, ErrorMessage = "Remark cannot exceed 100 characters.")]
        public string? Remark { get; set; }

        [Required(ErrorMessage = "ReferenceId is required.")]
        public string ReferenceId { get; set; } = Guid.NewGuid().ToString(); // Unique ID

        // Foreign Key for Beneficiary
        [Required(ErrorMessage = "Beneficiary is required.")]
        public int BeneficiaryId { get; set; }

        [Required(ErrorMessage = "Beneficiary reference is required.")]
        public Beneficiary Beneficiary { get; set; } = null!;

    }
}
