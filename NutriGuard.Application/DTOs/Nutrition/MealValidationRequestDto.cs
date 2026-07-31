using NutriGuard.Application.DTOs.Tracking;

namespace NutriGuard.Application.DTOs.Nutrition;

public class MealValidationRequestDto
{
    public List<CreateMealItemDto> MealItems { get; set; } = new();
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
