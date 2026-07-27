namespace NutriGuard.Application.DTOs.Recipe;

public class RecipeDetailsDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public int Servings { get; set; }

    public int PreparationTimeMinutes { get; set; }

    public List<RecipeIngredientDto> Ingredients { get; set; }
        = new();
}