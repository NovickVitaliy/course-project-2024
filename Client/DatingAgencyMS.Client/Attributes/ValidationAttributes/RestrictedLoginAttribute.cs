using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Attributes.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property)]
public class RestrictedLoginAttribute : ValidationAttribute
{
    private readonly string[] _restrictedLogins = [
        DbRoles.Guest.ToString(),
        DbRoles.Operator.ToString(),
        DbRoles.Admin.ToString(),
        DbRoles.Owner.ToString(),
    ];

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string login)
        {
            foreach (var restrictedLogin in _restrictedLogins)
            {
                if (string.Equals(login, restrictedLogin, StringComparison.OrdinalIgnoreCase))
                {
                    return new ValidationResult($"Логін {login} є зарезервованим словом");
                } 
            }
        }

        return ValidationResult.Success;
    }
}