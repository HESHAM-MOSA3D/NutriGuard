public static class AuthMessages
{
    public const string InvalidOtp = "Invalid OTP.";

    public const string OtpExpired = "OTP has expired.";

    public const string OtpVerified = "OTP verified successfully.";

    public const string OtpAlreadyVerified = "OTP has already been verified.";

    public const string TooManyAttempts = "Too many failed attempts.";

    public const string EmailSent = "If the email exists, an OTP has been sent.";

    public const string EmailSendingFailed = "Unable to send reset password email.";

    public const string PasswordReset = "Password has been reset successfully.";

    public const string PasswordMismatch = "Password and Confirm Password do not match.";

    public const string VerificationRequired = "OTP verification required.";

    public const string UnableToResetPassword = "Unable to reset password.";
}