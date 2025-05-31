using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models
{
    public class User
    {
        [Key]
        [Required(ErrorMessage = "User ID is required.")]
        [StringLength(50, ErrorMessage = "User ID can't be longer than 50 characters.")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100, ErrorMessage = "First Name can't be longer than 100 characters.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Middle Name is required.")]
        [StringLength(100, ErrorMessage = "Middle Name can't be longer than 100 characters.")]
        public required string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100, ErrorMessage = "Last Name can't be longer than 100 characters.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile Number must be numeric and 10 digits long.")]
        public required string MobileNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must contain '@' and a valid domain.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Aadhar Card Number is required.")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhar Card Number must be exactly 12 digits.")]
        public required string AadharCardNumber { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public required string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public required string State { get; set; }

        [Required(ErrorMessage = "Occupation Type is required.")]
        public required string OccupationType { get; set; }

        [Required(ErrorMessage = "Source of Income is required.")]
        public required string SourceOfIncome { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Account Number must be between 10 and 20 characters.")]
        public required string AccountNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password should be at least 6 characters long.")]
        public required string Password { get; set; }

        public int LoginAttempts { get; set; } = 0;

        public bool IsLocked { get; set; } = false;

        public Account? Account { get; set; }

        public string? TransactionPassword { get; set; }

        public List<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

        [Required(ErrorMessage = "Account Status is required.")]
        [StringLength(20, ErrorMessage = "Account Status can't be longer than 20 characters.")]
        public string AccountStatus { get; set; } = "Pending";
    }
}

