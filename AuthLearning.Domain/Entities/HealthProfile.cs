using NutriGuard.Domain.Enums;

namespace NutriGuard.Domain.Entities;

public class HealthProfile
{
    public int Id { get; set; }

    public double Height { get; set; }

    public double Weight { get; set; }

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public ActivityLevel ActivityLevel { get; set; }

    public DietType DietType { get; set; }

    public Goal Goal { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null;

}