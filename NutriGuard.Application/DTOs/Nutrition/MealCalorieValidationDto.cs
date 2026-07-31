namespace NutriGuard.Application.DTOs.Nutrition;

public class MealCalorieValidationDto
{
    public double MealCalories { get; set; }
    public double RemainingDailyCalories { get; set; }
    public double TargetDailyCalories { get; set; }
    public bool ExceedsRemainingBudget { get; set; }
    public bool IsHighCalorieMeal { get; set; }
}
