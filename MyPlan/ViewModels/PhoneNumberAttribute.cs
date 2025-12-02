using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MyPlan.ViewModels.Validation
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var phoneNumber = value.ToString();
            var pattern = @"^((\+9665\d{8})|(05\d{8}))$";

            if (!Regex.IsMatch(phoneNumber, pattern))
            {
                return new ValidationResult(
                    $"{validationContext.DisplayName} must be a valid phone number. Example: +966566193395 or 0566193395");
            }

            return ValidationResult.Success;
        }
    }

}
