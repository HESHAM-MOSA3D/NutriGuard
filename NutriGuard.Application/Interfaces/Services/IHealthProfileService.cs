using NutriGuard.Application.DTOs.HealthProfile;

namespace NutriGuard.Application.Interfaces.Services;

public interface IHealthProfileService
{
    Task<HealthProfileResponseDto> CreateAsync(
        string userId,
        CreateHealthProfileRequestDto request,
        CancellationToken cancellationToken = default);

    Task<HealthProfileResponseDto> GetAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<HealthProfileResponseDto> UpdateAsync(
        string userId,
        UpdateHealthProfileRequestDto request,
        CancellationToken cancellationToken = default);
}