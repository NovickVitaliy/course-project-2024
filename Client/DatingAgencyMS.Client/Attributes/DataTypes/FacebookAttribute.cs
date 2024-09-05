using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DatingAgencyMS.Client.Attributes.DataTypes;

public class FacebookAttribute : DataTypeAttribute
{
    private const string FacebookPattern = @"^(https?:\/\/)?(www\.)?facebook\.com\/.+$";

    public FacebookAttribute() : base(DataType.Url)
    {
        ErrorMessage = "Неправильне значення посилання на Facebook";
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true;
        }

        return value is string stringValue 
               && (Regex.IsMatch(stringValue, FacebookPattern) || string.IsNullOrWhiteSpace(stringValue));
    }
}