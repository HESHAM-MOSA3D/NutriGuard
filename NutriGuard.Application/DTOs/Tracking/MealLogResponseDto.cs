namespace NutriGuard.Application.DTOs.Tracking;

public class MealLogResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public MealLogDto? Data { get; set; }
    public MealValidationResult? ValidationResult { get; set; }

    public static MealLogResponseDto Success(MealLogDto data, MealValidationResult? validationResult = null)
    {
        return new()
        {
            IsSuccess = true,
            Message = "Success",
            Data = data,
            ValidationResult = validationResult
        };
    }

    public static MealLogResponseDto Failure(string message, MealValidationResult? validationResult = null)
    {
        return new()
        {
            IsSuccess = false,
            Message = message,
            ValidationResult = validationResult
        };
    }
}
