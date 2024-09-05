using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Attributes.DataTypes;

[AttributeUsage(AttributeTargets.Property)]
public class TelegramAttribute : DataTypeAttribute
{
    public TelegramAttribute() : base(DataType.Custom)
    {
        ErrorMessage = "Неправильне значення Id телеграм акаунта. Значення повинне починатися зі знаку @";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return value is string telegramId
               && (telegramId.StartsWith('@') || string.IsNullOrWhiteSpace(telegramId));
    }
}