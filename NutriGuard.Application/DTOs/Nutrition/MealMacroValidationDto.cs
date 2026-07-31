namespace NutriGuard.Application.DTOs.Nutrition;

public class MealMacroValidationDto
{
    public double MealProteinGrams { get; set; }
    public double MealCarbsGrams { get; set; }
    public double MealFatGrams { get; set; }

    public double RemainingProteinGrams { get; set; }
    public double RemainingCarbsGrams { get; set; }
    public double RemainingFatGrams { get; set; }

    public bool ExceedsProteinBudget { get; set; }
    public bool ExceedsCarbsBudget { get; set; }
    public bool ExceedsFatBudget { get; set; }

    public bool IsLowProtein { get; set; }
    public bool IsHighCarb { get; set; }
    public bool IsHighFat { get; set; }
}
