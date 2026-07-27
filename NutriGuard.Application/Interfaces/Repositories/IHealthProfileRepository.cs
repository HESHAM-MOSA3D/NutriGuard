using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IHealthProfileRepository
    : IGenericRepository<HealthProfile>
{
    Task<HealthProfile?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);


   
}