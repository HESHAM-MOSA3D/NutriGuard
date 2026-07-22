using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}