namespace NutriGuard.Application.DTOs.HealthProfile;

public sealed class HealthProfileResponseDto
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public HealthProfileDto? Data { get; init; }

    private HealthProfileResponseDto()
    {
    }

    public static HealthProfileResponseDto Success(
        HealthProfileDto data,
        string message = "Success")
    {
        return new HealthProfileResponseDto
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static HealthProfileResponseDto Failure(string message)
    {
        return new HealthProfileResponseDto
        {
            IsSuccess = false,
            Message = message
        };
    }
}