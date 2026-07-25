namespace NutriGuard.Application.Interfaces.Services;

public interface IFoodImportService
{
    Task SeedFoodsAsync(CancellationToken cancellationToken = default);
}