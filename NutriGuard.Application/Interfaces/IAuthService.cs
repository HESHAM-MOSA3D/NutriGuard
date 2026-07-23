using NutriGuard.Application.DTOs.Auth;

namespace NutriGuard.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

        Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);

        Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request);

        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
    }
}