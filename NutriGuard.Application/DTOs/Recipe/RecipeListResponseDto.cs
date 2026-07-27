namespace NutriGuard.Application.DTOs.Recipe;

public class RecipeListResponseDto
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<RecipeDto>? Data { get; set; }

    public int TotalCount { get; set; }

    public static RecipeListResponseDto Success(
        List<RecipeDto> data,
        int totalCount)
    {
        return new()
        {
            IsSuccess = true,
            Message = "Success",
            Data = data,
            TotalCount = totalCount
        };
    }

    public static RecipeListResponseDto Failure(string message)
    {
        return new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}