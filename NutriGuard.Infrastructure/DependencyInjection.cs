using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NutriGuard.Application.Interfaces;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Application.Services;
using NutriGuard.Application.Validators.Foods;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Csv;
using NutriGuard.Infrastructure.Persistence;

using NutriGuard.Infrastructure.Repositories;
using NutriGuard.Infrastructure.Security;
using NutriGuard.Infrastructure.Services;
using System.Text;



namespace NutriGuard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection 
    AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IEmailService, EmailService>();

        services.AddDbContext<AppDbContext>(options =>
           options.UseNpgsql(
    configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IFoodImportService, FoodImportService>();
        services.AddScoped<IFoodService, FoodService>();

        services.AddScoped<IHealthProfileRepository, HealthProfileRepository>();
       // services.AddScoped<IFoodPreferenceRepository, FoodPreferenceRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<INutritionCalculatorService, NutritionCalculatorService>();

        services.AddScoped<IHealthProfileService, HealthProfileService>();
        services.AddScoped<IFoodPreferenceService, FoodPreferenceService>();
        services.AddScoped<ITrackingService, TrackingService>();

        services.AddScoped<IFoodRepository, FoodRepository>();

        services.AddScoped<IFoodPreferenceRepository, FoodPreferenceRepository>();
        services.AddScoped<IMealLogRepository, MealLogRepository>();
        services.AddScoped<IWaterLogRepository, WaterLogRepository>();
        services.AddScoped<IWeightLogRepository, WeightLogRepository>();


        services.AddScoped<IRecipeImportService, RecipeImportService>();

        services.AddValidatorsFromAssemblyContaining<FoodSearchRequestValidator>();


        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeService, RecipeService>();

        services.AddScoped<DatabaseSeeder>();

        services.AddValidatorsFromAssemblyContaining<FoodSearchRequestValidator>();

        services.AddScoped<IFoodUnitConversionRepository,FoodUnitConversionRepository>();

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = false;
        })

        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
.AddJwtBearer(options =>
{
    options.SaveToken = true;

    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),

        ClockSkew = TimeSpan.Zero
    };
});

        

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        return services;
    }
}