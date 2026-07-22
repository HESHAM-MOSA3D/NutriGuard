using Microsoft.AspNetCore.Identity;

namespace NutriGuard.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        // Navigate
        public HealthProfile? HealthProfile { get; set; }
    }
}