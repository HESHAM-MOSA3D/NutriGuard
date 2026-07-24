using NutriGuard.Domain.Enums;

namespace NutriGuard.Domain.Entities;

public class FoodPreference
{
    public int Id { get; set; }

    public int HealthProfileId { get; set; }
    public HealthProfile HealthProfile { get; set; } = null!;

    public int FoodId { get; set; }

    public Food Food { get; set; } = null!;

    public FoodPreferenceType PreferenceType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}