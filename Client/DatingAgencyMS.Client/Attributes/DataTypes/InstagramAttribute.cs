using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DatingAgencyMS.Client.Attributes.DataTypes;

public class InstagramAttribute : DataTypeAttribute
{
    private const string InstagramPattern = @"^(https?:\/\/)?(www\.)?instagram\.com\/.+$";

    public InstagramAttribute() : base(DataType.Url)
    {
        ErrorMessage = "Неправильне посилання на Instagram.";
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true; 
        }

        return value is string stringValue 
               && (Regex.IsMatch(stringValue, InstagramPattern) || string.IsNullOrWhiteSpace(stringValue));
    }
}