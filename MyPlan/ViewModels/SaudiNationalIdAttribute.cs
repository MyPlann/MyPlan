using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.Validation
{
    public class SaudiNationalIdAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var nationalId = value.ToString();

            if (nationalId.Length != 10)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be 10 digits.");
            }

            var regionCode = nationalId[0];
            var validRegionCodes = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            if (!validRegionCodes.Contains(regionCode))
            {
                return new ValidationResult($"{validationContext.DisplayName} is not a valid Saudi National ID.");
            }

            return ValidationResult.Success;
        }
    }
}
