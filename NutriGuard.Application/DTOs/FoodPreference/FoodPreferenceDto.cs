using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.DTOs.FoodPreference;

public sealed class FoodPreferenceDto
{
    public int Id { get; init; }

    public int FoodId { get; init; }

    public string FoodName { get; init; } = string.Empty;

    public FoodPreferenceType PreferenceType { get; init; }

    public DateTime CreatedAt { get; init; }
}