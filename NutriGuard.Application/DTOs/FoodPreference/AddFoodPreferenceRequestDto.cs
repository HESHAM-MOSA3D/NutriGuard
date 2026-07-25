using System.ComponentModel.DataAnnotations;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.DTOs.FoodPreference;

public sealed class AddFoodPreferenceRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "A valid FoodId is required.")]
    public int FoodId { get; init; }

    [EnumDataType(typeof(FoodPreferenceType))]
    public FoodPreferenceType PreferenceType { get; init; }
}