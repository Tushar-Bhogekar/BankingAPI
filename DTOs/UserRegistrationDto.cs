using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "User ID must be between 6 and 16 characters.")]
        public required string UserId { get; set; }


        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 6 and 100 characters.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Middle Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Middle Name must be between 6 and 100 characters.")]
        public required string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 6 and 100 characters.")]
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
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhar Card Number must be numeric and 12 digits long.")]
        public required string AadharCardNumber { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DateOfBirthValidation]
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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Password should be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public required string Password { get; set; }

    }

    public class AccountStatusUpdateDto
    {
        [Required]
        public string? UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string? AccountStatus { get; set; } // Example: "Active", "Locked", "Pending"
    }

    public class DateOfBirthValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly dateOfBirth)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);

                if (dateOfBirth >= today)
                {
                    return new ValidationResult("Date of Birth cannot be today or a future date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}


