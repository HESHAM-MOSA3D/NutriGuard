using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriGuard.Application.Interfaces.Services
{
    public interface IRecipeImportService
    {
        Task SeedRecipesAsync(CancellationToken cancellationToken = default);
    }
}
