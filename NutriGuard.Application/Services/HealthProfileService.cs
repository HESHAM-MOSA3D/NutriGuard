using NutriGuard.Application.DTOs.HealthProfile;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Services;

public class HealthProfileService : IHealthProfileService
{
    private readonly IHealthProfileRepository _healthProfileRepository;

    public HealthProfileService(
        IHealthProfileRepository healthProfileRepository)
    {
        _healthProfileRepository = healthProfileRepository;
    }

    public async Task<HealthProfileResponseDto> CreateAsync(
        string userId,
        CreateHealthProfileRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var exists = await _healthProfileRepository.ExistsAsync(
            x => x.UserId == userId,
            cancellationToken);

        if (exists)
        {
            return HealthProfileResponseDto.Failure(
                "Health profile already exists.");
        }

        var profile = new HealthProfile
        {
            UserId = userId,
            Height = request.Height,
            Weight = request.Weight,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            ActivityLevel = request.ActivityLevel,
            DietType = request.DietType,
            Goal = request.Goal
        };

        await _healthProfileRepository.AddAsync(profile, cancellationToken);
        await _healthProfileRepository.SaveChangesAsync(cancellationToken);

        var dto = new HealthProfileDto
        {
            Height = profile.Height,
            Weight = profile.Weight,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            ActivityLevel = profile.ActivityLevel,
            DietType = profile.DietType,
            Goal = profile.Goal
        };

        return HealthProfileResponseDto.Success(
            dto,
            "Health profile created successfully.");
    }

    public async Task<HealthProfileResponseDto> GetAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _healthProfileRepository.FirstOrDefaultAsync(
            x => x.UserId == userId,
            cancellationToken);

        if (profile is null)
        {
            return HealthProfileResponseDto.Failure(
                "Health profile not found.");
        }

        var dto = new HealthProfileDto
        {
            Height = profile.Height,
            Weight = profile.Weight,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            ActivityLevel = profile.ActivityLevel,
            DietType = profile.DietType,
            Goal = profile.Goal
        };

        return HealthProfileResponseDto.Success(
            dto,
            "Health profile retrieved successfully.");
    }

    public async Task<HealthProfileResponseDto> UpdateAsync(
        string userId,
        UpdateHealthProfileRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var profile = await _healthProfileRepository.FirstOrDefaultAsync(
            x => x.UserId == userId,
            cancellationToken);

        if (profile is null)
        {
            return HealthProfileResponseDto.Failure(
                "Health profile not found.");
        }

        profile.Height = request.Height;
        profile.Weight = request.Weight;
        profile.DateOfBirth = request.DateOfBirth;
        profile.Gender = request.Gender;
        profile.ActivityLevel = request.ActivityLevel;
        profile.DietType = request.DietType;
        profile.Goal = request.Goal;

        _healthProfileRepository.Update(profile);

        await _healthProfileRepository.SaveChangesAsync(cancellationToken);

        var dto = new HealthProfileDto
        {
            Height = profile.Height,
            Weight = profile.Weight,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            ActivityLevel = profile.ActivityLevel,
            DietType = profile.DietType,
            Goal = profile.Goal
        };

        return HealthProfileResponseDto.Success(
            dto,
            "Health profile updated successfully.");
    }

    public async Task<HealthProfileResponseDto> DeleteAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _healthProfileRepository.FirstOrDefaultAsync(
            x => x.UserId == userId,
            cancellationToken);

        if (profile is null)
        {
            return HealthProfileResponseDto.Failure(
                "Health profile not found.");
        }

        _healthProfileRepository.Delete(profile);

        await _healthProfileRepository.SaveChangesAsync(cancellationToken);

        return HealthProfileResponseDto.Success(
            null!,
            "Health profile deleted successfully.");
    }



    public async Task<ProfileCompletionResponseDto> GetCompletionStatusAsync(
    string userId,
    CancellationToken cancellationToken = default)
    {
        var profile = await _healthProfileRepository
            .GetByUserIdAsync(userId, cancellationToken);

        if (profile is null)
        {
            return ProfileCompletionResponseDto.Failure(
                "Health profile not found.");
        }

        var missingFields = new List<string>();

        if (profile.Height <= 0)
            missingFields.Add("Height");

        if (profile.Weight <= 0)
            missingFields.Add("Weight");

        if (profile.DateOfBirth == default)
            missingFields.Add("DateOfBirth");

        if (profile.Gender == 0)
            missingFields.Add("Gender");

        if (profile.ActivityLevel == 0)
            missingFields.Add("ActivityLevel");

        if (profile.DietType == 0)
            missingFields.Add("DietType");

        if (profile.Goal == 0)
            missingFields.Add("Goal");

        const int totalFields = 7;

        var completedFields = totalFields - missingFields.Count;

        var percentage = (int)Math.Round(
            completedFields * 100.0 / totalFields);

        return ProfileCompletionResponseDto.Success(
            new ProfileCompletionDto
            {
                CompletionPercentage = percentage,
                IsCompleted = percentage == 100,
                MissingFields = missingFields
            });
    }
}