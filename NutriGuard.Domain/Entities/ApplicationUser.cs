using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace NutriGuard.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigate
        public HealthProfile? HealthProfile { get; set; }
        public ICollection<MealLog> MealLogs { get; set; } = new List<MealLog>();
        public ICollection<WaterLog> WaterLogs { get; set; } = new List<WaterLog>();
        public ICollection<WeightLog> WeightLogs { get; set; } = new List<WeightLog>();
    }
}