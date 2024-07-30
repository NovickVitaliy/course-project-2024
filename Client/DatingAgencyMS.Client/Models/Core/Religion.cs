namespace DatingAgencyMS.Client.Models.Core;

public enum Religion
{
    Christianity,
    Islam,
    Buddhism,
    Hinduism,
    Judaism,
    Other
}

public static class ReligionHelper
{
    public static string GetUkrainianTranslation(Religion religion)
    {
        return religion switch
        {
            Religion.Christianity => "Християнство",
            Religion.Islam => "Іслам",
            Religion.Buddhism => "Буддизм",
            Religion.Hinduism => "Індуїзм",
            Religion.Judaism => "Юдаїзм",
            Religion.Other => "Інше",
            _ => throw new ArgumentException("Invalid religion value", nameof(religion))
        };
    }

    public static Religion GetReligionFromUkrainian(string ukrainian)
    {
        return ukrainian switch
        {
            "Християнство" => Religion.Christianity,
            "Іслам" => Religion.Islam,
            "Буддизм" => Religion.Buddhism,
            "Індуїзм" => Religion.Hinduism,
            "Юдаїзм" => Religion.Judaism,
            "Інше" => Religion.Other,
            _ => throw new ArgumentException("Invalid Ukrainian translation", nameof(ukrainian))
        };
    }
}