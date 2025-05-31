
namespace BankingAPI.Models
{
    public class UserUpdateDto
    {
        public required string UserId { get; set; } // This is mandatory to identify the user
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? OccupationType { get; set; }
        public string? SourceOfIncome { get; set; }
    }

}