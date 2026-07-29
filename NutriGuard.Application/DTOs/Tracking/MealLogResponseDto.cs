namespace NutriGuard.Application.DTOs.Tracking;

public class MealLogResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public MealLogDto? Data { get; set; }

    public static MealLogResponseDto Success(MealLogDto data)
    {
        return new()
        {
            IsSuccess = true,
            Message = "Success",
            Data = data
        };
    }

    public static MealLogResponseDto Failure(string message)
    {
        return new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
