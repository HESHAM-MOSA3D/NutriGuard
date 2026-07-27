namespace NutriGuard.Application.DTOs.FoodPreference;

using NutriGuard.Domain.Enums;

public sealed class UpdateFoodPreferenceRequestDto
{
    public FoodPreferenceType PreferenceType { get; init; }
}