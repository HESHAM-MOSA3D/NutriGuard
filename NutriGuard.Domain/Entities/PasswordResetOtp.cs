namespace NutriGuard.Domain.Entities;

public class PasswordResetOtp
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public string OtpCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public int FailedAttempts { get; set; }

    public bool IsVerified { get; set; }
}