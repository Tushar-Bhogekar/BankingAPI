using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankingAPI.Models
{
    public class Beneficiary
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Account Number must be between 10 and 20 characters.")]
        public required string AccountNumber { get; set; }

        [StringLength(50, ErrorMessage = "Nickname cannot exceed 50 characters.")]
        public string? Nickname { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } // Assuming you have a `User` model

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
