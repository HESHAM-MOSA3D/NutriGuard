namespace NutriGuard.Application.DTOs.Recipe;

public class RecipeSearchResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Servings { get; set; }

    public int PreparationTimeMinutes { get; set; }

    public List<string> Aliases { get; set; } = [];

    public List<RecipeIngredientDto> Ingredients { get; set; } = [];
}