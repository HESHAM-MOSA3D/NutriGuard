namespace NutriGuard.Application.DTOs.Nutrition;

public class RecipeEligibilityDto
{
    public int RecipeId { get; set; }
    public string RecipeTitle { get; set; } = string.Empty;
    public bool IsEligible { get; set; }
    public List<string> IneligibilityReasons { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool IsTraditionalEgyptian { get; set; }
}
