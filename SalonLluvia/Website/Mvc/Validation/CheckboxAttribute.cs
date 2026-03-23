using Mvc.Models.Gallery.ViewModels.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Validation;

public class CheckboxAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
        // We expect a cast exception if a non-string was passed in.
        if (value is null)
        {
            return ValidationResult.Success;
        }

        IEnumerable<ICheckbox> checkboxes = (IEnumerable<ICheckbox>)value;

        if (!checkboxes.Any(checkbox => checkbox.IsChecked))
        {
            return new ValidationResult(GetMessage());
        }

        return ValidationResult.Success;
    }

    private string GetMessage()
    {
        return base.ErrorMessage ?? $"Please enter at least one checkbox.";
    }
}