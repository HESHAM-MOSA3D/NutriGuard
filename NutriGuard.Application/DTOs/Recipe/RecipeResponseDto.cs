namespace NutriGuard.Application.DTOs.Recipe;

public class RecipeResponseDto
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = string.Empty;

    public RecipeDetailsDto? Data { get; set; }

    public static RecipeResponseDto Success(
        RecipeDetailsDto data)
    {
        return new()
        {
            IsSuccess = true,
            Message = "Success",
            Data = data
        };
    }

    public static RecipeResponseDto Failure(string message)
    {
        return new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}