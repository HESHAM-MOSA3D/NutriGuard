using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Application.Settings;
using NutriGuard.Infrastructure;
using NutriGuard.Infrastructure.Persistence;
using NutriGuard.Infrastructure.Persistence.Seed;
using NutriGuard.Infrastructure.Services;
using Swashbuckle.AspNetCore;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<SendGridSettings>(
    builder.Configuration.GetSection("SendGrid"));


builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "NutriGuard API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});


var app = builder.Build();



//using (var scope = app.Services.CreateScope())
//{
//    var seeder = scope.ServiceProvider
//        .GetRequiredService<DatabaseSeeder>();

//    await seeder.SeedAsync();
//}

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    // Food Unit Conversions
//    var unitSeeder = new FoodUnitConversionSeeder(context);

//    await unitSeeder.SeedAsync(
//        Path.Combine(
//            app.Environment.ContentRootPath,
//            "SeedData",
//            "FoodUnitConversions.csv"));

//    // Food Tags
//    var tagSeeder = new FoodTagSeeder(context);

//    await tagSeeder.SeedAsync(
//        Path.Combine(
//            app.Environment.ContentRootPath,
//            "SeedData",
//            "FoodTags.csv"));

//    // Food Tag Assignments
//    var assignmentSeeder = new FoodTagAssignmentSeeder(context);

//    await assignmentSeeder.SeedAsync(
//        Path.Combine(
//            app.Environment.ContentRootPath,
//            "SeedData",
//            "FoodTagAssignments.csv"));
//}


app.UseSwagger();

app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
