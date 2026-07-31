namespace NutriGuard.Application.DTOs.Nutrition;

public class FoodEligibilityDto
{
    public int FoodId { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public bool IsEligible { get; set; }
    public List<string> IneligibilityReasons { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> HealthTags { get; set; } = new();
    public bool IsTraditionalEgyptian { get; set; }
}
