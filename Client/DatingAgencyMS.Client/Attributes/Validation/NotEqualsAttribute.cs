using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DatingAgencyMS.Client.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class NotEqualsAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public NotEqualsAttribute(string otherProperty)
    {
        _otherProperty = otherProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var propertyInfo = validationContext.ObjectType.GetRuntimeProperty(_otherProperty);
        if (propertyInfo is null)
        {
            return new ValidationResult($"Властивість з ім'ям {_otherProperty} відсутня");
        }

        var otherPropertyValue = propertyInfo.GetValue(validationContext.ObjectInstance);
        return Equals(value, otherPropertyValue)
            ? new ValidationResult($"Властивість не може бути рівна {otherPropertyValue}")
            : ValidationResult.Success;
    }
}