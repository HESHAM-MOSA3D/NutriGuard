using Moq;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Services;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;
using Xunit;

namespace NutriGuard.Tests;

public class HealthProfileServiceTests
{
    private readonly Mock<IHealthProfileRepository> _healthProfileRepositoryMock;
    private readonly HealthProfileService _service;

    public HealthProfileServiceTests()
    {
        _healthProfileRepositoryMock = new Mock<IHealthProfileRepository>();
        _service = new HealthProfileService(_healthProfileRepositoryMock.Object);
    }

    [Fact]
    public async Task GetCompletionStatusAsync_WhenNoFoodPreferences_IncludesFoodPreferencesInMissingFields()
    {
        // Arrange
        var userId = "test-user-123";
        var profile = new HealthProfile
        {
            Id = 1,
            UserId = userId,
            Height = 175,
            Weight = 70,
            DateOfBirth = new DateOnly(1995, 5, 15),
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive,
            DietType = DietType.Balanced,
            Goal = Goal.LoseWeight,
            FoodPreferences = new List<FoodPreference>() // Empty food preferences
        };

        _healthProfileRepositoryMock
            .Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        // Act
        var response = await _service.GetCompletionStatusAsync(userId);

        // Assert
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Contains("FoodPreferences", response.Data.MissingFields);
        Assert.False(response.Data.IsCompleted);
        Assert.Equal(88, response.Data.CompletionPercentage);
    }

    [Fact]
    public async Task GetCompletionStatusAsync_WhenFoodPreferencesExist_RemovesFoodPreferencesFromMissingFieldsAndCalculates100Percent()
    {
        // Arrange
        var userId = "test-user-123";
        var profile = new HealthProfile
        {
            Id = 1,
            UserId = userId,
            Height = 175,
            Weight = 70,
            DateOfBirth = new DateOnly(1995, 5, 15),
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive,
            DietType = DietType.Balanced,
            Goal = Goal.LoseWeight,
            FoodPreferences = new List<FoodPreference>
            {
                new FoodPreference
                {
                    Id = 1,
                    HealthProfileId = 1,
                    FoodId = 10,
                    PreferenceType = FoodPreferenceType.Dislike,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        _healthProfileRepositoryMock
            .Setup(repo => repo.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        // Act
        var response = await _service.GetCompletionStatusAsync(userId);

        // Assert
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.DoesNotContain("FoodPreferences", response.Data.MissingFields);
        Assert.Empty(response.Data.MissingFields);
        Assert.True(response.Data.IsCompleted);
        Assert.Equal(100, response.Data.CompletionPercentage);
    }
}
