namespace NutriGuard.Application.DTOs.Food;

public sealed class FoodSearchRequestDto
{

    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }


    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; } = false;


    //public string Query { get; set; } = string.Empty;
}