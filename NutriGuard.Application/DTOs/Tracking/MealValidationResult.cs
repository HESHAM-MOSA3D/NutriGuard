using NutriGuard.Application.DTOs.Nutrition;

namespace NutriGuard.Application.DTOs.Tracking;

public class MealValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public bool IsRecommended { get; set; } = true;

    private List<string> _errors = new();
    private List<string> _warnings = new();
    private List<string> _tips = new();

    public List<string> Errors => _errors.Distinct().ToList();
    public List<string> Warnings => _warnings.Distinct().ToList();
    public List<string> Tips => _tips.Distinct().ToList();

    public MealCalorieValidationDto? CalorieBreakdown { get; set; }

    public MealMacroValidationDto? MacroBreakdown { get; set; }

    public void AddError(string error)
    {
        _errors.Add(error);
    }

    public void AddWarning(string warning)
    {
        _warnings.Add(warning);
    }

    public void AddTip(string tip)
    {
        _tips.Add(tip);
    }
}