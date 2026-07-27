using System.ComponentModel.DataAnnotations;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.DTOs.HealthProfile;

public class CreateHealthProfileRequestDto : IValidatableObject
{
    [Range(50, 250, ErrorMessage = "Height must be between 50 and 250 cm.")]
    public double Height { get; set; }

    [Range(20, 500, ErrorMessage = "Weight must be between 20 and 500 kg.")]
    public double Weight { get; set; }

    [Required]
    public DateOnly DateOfBirth { get; set; }
  //  [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }

    //[EnumDataType(typeof(ActivityLevel))]
    public ActivityLevel? ActivityLevel { get; set; }

   // [EnumDataType(typeof(DietType))]
    public DietType? DietType { get; set; }

    [EnumDataType(typeof(Goal))]
    public Goal ?Goal { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (DateOfBirth > today)
        {
            yield return new ValidationResult(
                "Date of birth cannot be in the future.",
                new[] { nameof(DateOfBirth) });

            yield break;
        }

        var age = today.Year - DateOfBirth.Year;

        if (DateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        if (age < 18 || age > 120)
        {
            yield return new ValidationResult(
                "Age must be between 18 and 120 years.",
                new[] { nameof(DateOfBirth) });
        }
    }
}