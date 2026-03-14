using EmailValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Validation;

public sealed class EmailAttribute : ValidationAttribute, IClientModelValidator
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
        // We expect a cast exception if a non-string was passed in.
        if (value is null)
        {
            return ValidationResult.Success;
        }

        string email = (string)value;
        if (!EmailValidator.Validate(email, allowInternational: true))
        {
            return new ValidationResult(GetMessage());
        }

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext ctx)
    {
        if (!ctx.Attributes.ContainsKey("data-val"))
        {
            ctx.Attributes.Add("data-val", "true");
        }

        ctx.Attributes.Add("data-val-email", GetMessage());
    }

    private string GetMessage()
    {
        return base.ErrorMessage ?? $"Please enter a valid email";
    }
}