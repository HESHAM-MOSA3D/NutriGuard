using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.DTOs.FoodPreference;
using NutriGuard.Application.Interfaces.Services;
using System.Security.Claims;

namespace NutriGuard.API.Controllers;

[ApiController]
[Authorize]
[Route("api/food-preferences")]
public class FoodPreferenceController : ControllerBase
{
    private readonly IFoodPreferenceService _foodPreferenceService;
    private readonly IHealthProfileService _healthProfileService;

    public FoodPreferenceController(
        IFoodPreferenceService foodPreferenceService,
        IHealthProfileService healthProfileService)
    {
        _foodPreferenceService = foodPreferenceService;
        _healthProfileService = healthProfileService;
    }



    [HttpPost]
    public async Task<IActionResult> Add(
     [FromBody] AddFoodPreferenceRequestDto request,
     CancellationToken cancellationToken)
    {
        var healthProfileId = await GetHealthProfileIdAsync(cancellationToken);

        if (healthProfileId is null)
            return NotFound("Health profile not found.");

        var response = await _foodPreferenceService.AddAsync(
            healthProfileId.Value,
            request,
            cancellationToken);

        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }
    [HttpDelete("{foodId:int}")]
    public async Task<IActionResult> Delete(
     int foodId,
     CancellationToken cancellationToken)
    {
        var healthProfileId = await GetHealthProfileIdAsync(cancellationToken);

        if (healthProfileId is null)
            return NotFound("Health profile not found.");

        var response = await _foodPreferenceService.RemoveAsync(
            healthProfileId.Value,
            foodId,
            cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : NotFound(response);
    }





    [HttpPut("{foodId:int}")]
    public async Task<IActionResult> Update(
    int foodId,
    [FromBody] UpdateFoodPreferenceRequestDto request,
    CancellationToken cancellationToken)
    {
        var healthProfileId = await GetHealthProfileIdAsync(cancellationToken);

        if (healthProfileId is null)
            return NotFound("Health profile not found.");

        var response = await _foodPreferenceService.UpdateAsync(
            healthProfileId.Value,
            foodId,
            request,
            cancellationToken);

        return response.IsSuccess
            ? Ok(response)
            : BadRequest(response);
    }





    [HttpGet]
    public async Task<IActionResult> Get(
    CancellationToken cancellationToken)
    {
        var healthProfileId = await GetHealthProfileIdAsync(cancellationToken);

        if (healthProfileId is null)
            return NotFound("Health profile not found.");

        var response = await _foodPreferenceService.GetByHealthProfileIdAsync(
            healthProfileId.Value,
            cancellationToken);

        return response.IsSuccess
            ? Ok(response)
            : NotFound(response);
    }





    private async Task<int?> GetHealthProfileIdAsync(
    CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return null;

        var profile = await _healthProfileService.GetAsync(
            userId,
            cancellationToken);

        if (!profile.IsSuccess || profile.Data is null)
            return null;

        return profile.Data.Id;
    }
}