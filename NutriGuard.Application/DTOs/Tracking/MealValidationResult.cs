using NutriGuard.Application.DTOs.Nutrition;

namespace NutriGuard.Application.DTOs.Tracking;

public class MealValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<string> Errors { get; set; } = new();

    public List<string> Warnings { get; set; } = new();

    public List<string> Tips { get; set; } = new();

    public MealCalorieValidationDto? CalorieBreakdown { get; set; }

    public MealMacroValidationDto? MacroBreakdown { get; set; }

    public void AddError(string error)
    {
        Errors.Add(error);
    }

    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }

    public void AddTip(string tip)
    {
        Tips.Add(tip);
    }
}