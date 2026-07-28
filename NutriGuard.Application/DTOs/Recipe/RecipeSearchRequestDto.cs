namespace NutriGuard.Application.DTOs.Recipe;

public class RecipeSearchRequestDto
{
    public string? SearchTerm { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; }
}