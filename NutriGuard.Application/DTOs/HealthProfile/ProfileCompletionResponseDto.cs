namespace NutriGuard.Application.DTOs.HealthProfile;

public sealed class ProfileCompletionResponseDto
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public ProfileCompletionDto? Data { get; init; }

    private ProfileCompletionResponseDto()
    {
    }

    public static ProfileCompletionResponseDto Success(
        ProfileCompletionDto data,
        string message = "Success")
    {
        return new ProfileCompletionResponseDto
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static ProfileCompletionResponseDto Failure(
        string message)
    {
        return new ProfileCompletionResponseDto
        {
            IsSuccess = false,
            Message = message
        };
    }
}