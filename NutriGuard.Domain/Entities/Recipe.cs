namespace NutriGuard.Domain.Entities;

public class Recipe
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public int Servings { get; set; }

    public int PreparationTimeMinutes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        = new List<RecipeIngredient>();
}