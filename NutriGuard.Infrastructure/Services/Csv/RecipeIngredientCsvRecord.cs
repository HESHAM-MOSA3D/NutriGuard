namespace NutriGuard.Infrastructure.Csv;

public class RecipeIngredientCsvRecord
{
    public string RecipeName { get; set; } = string.Empty;

    public string FoodName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;
}