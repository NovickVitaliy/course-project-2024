using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DatingAgencyMS.Client.Attributes.DataTypes;

public class TikTokAttribute : DataTypeAttribute
{
    private const string TiktokPattern = @"^(https?:\/\/)?(www\.)?tiktok\.com\/.+$";

    public TikTokAttribute() : base(DataType.Url)
    {
        ErrorMessage = "Неправильне посилання на TikTok.";
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true; 
        }

        return value is string stringValue 
               && (Regex.IsMatch(stringValue, TiktokPattern) || string.IsNullOrWhiteSpace(stringValue));
    }
}