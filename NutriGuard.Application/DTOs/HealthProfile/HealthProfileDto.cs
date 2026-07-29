using NutriGuard.Application.DTOs.FoodPreference;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.DTOs.HealthProfile;

public class HealthProfileDto
{
        public int Id { get; set; }

    public double Height { get; set; }

    public double Weight { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public ActivityLevel? ActivityLevel { get; set; }

    public DietType? DietType { get; set; }

    public Goal? Goal { get; set; }

    public List<FoodPreferenceDto> FoodPreferences { get; set; } = new();
}
