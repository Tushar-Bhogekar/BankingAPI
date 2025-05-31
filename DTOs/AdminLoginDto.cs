public class AdminLoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class LoginRequest
{
    public string UserId { get; set; }
    public string Password { get; set; }
}

public class AccountNumberDto
{
    public string AccountNumber { get; set; }
}

public class VerifyOtpRequest
{
    public string Otp { get; set; }
}

public class ForgotPasswordRequest
{
    public string UserId { get; set; }
}

public class VerifyOtpForPasswordResetRequest
{
    public string Otp { get; set; }
    public string UserId { get; set; }
}

public class ResetPasswordRequest
{
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public string Otp { get; set; }  // Ensure OTP is included in the request
}
