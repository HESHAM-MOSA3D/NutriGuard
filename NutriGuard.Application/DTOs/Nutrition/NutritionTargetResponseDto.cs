namespace NutriGuard.Application.DTOs.Nutrition;

public sealed class NutritionTargetResponseDto
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public NutritionTargetDto? Data { get; init; }

    private NutritionTargetResponseDto()
    {
    }

    public static NutritionTargetResponseDto Success(
        NutritionTargetDto data,
        string message = "Success")
    {
        return new NutritionTargetResponseDto
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static NutritionTargetResponseDto Failure(
        string message)
    {
        return new NutritionTargetResponseDto
        {
            IsSuccess = false,
            Message = message
        };
    }
}