namespace NutriGuard.Application.DTOs.Recipe;

public class RecipeIngredientDto
{
    public int FoodId { get; set; }

    public string FoodName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;
}