namespace NutriGuard.Application.DTOs.Recipe;

public class RecipeDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Servings { get; set; }

    public int PreparationTimeMinutes { get; set; }
}