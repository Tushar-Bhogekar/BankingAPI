public class Otp
{
    public int Id { get; set; }
    public required string AccountNumber { get; set; }
    public required string OTP { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVerified { get; set; }
}
