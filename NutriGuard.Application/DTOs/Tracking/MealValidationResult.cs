namespace NutriGuard.Application.DTOs.Tracking;

public class MealValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<string> Errors { get; set; } = new();

    public List<string> Warnings { get; set; } = new();

    public void AddError(string error)
    {
        Errors.Add(error);
    }

    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
}