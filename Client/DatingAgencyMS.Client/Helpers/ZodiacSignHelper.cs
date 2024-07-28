using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Helpers;

public static class ZodiacSignHelper
{
    public static string GetUkrainianTranslation(ZodiacSign zodiacSign) =>
        zodiacSign switch
        {
            ZodiacSign.Aries => "Овен",
            ZodiacSign.Taurus => "Телець",
            ZodiacSign.Gemini => "Близнюки",
            ZodiacSign.Cancer => "Рак",
            ZodiacSign.Leo => "Лев",
            ZodiacSign.Virgo => "Діва",
            ZodiacSign.Libra => "Терези",
            ZodiacSign.Scorpio => "Скорпіон",
            ZodiacSign.Sagittarius => "Стрілець",
            ZodiacSign.Capricorn => "Козеріг",
            ZodiacSign.Aquarius => "Водолій",
            ZodiacSign.Pisces => "Риби",
            _ => throw new ArgumentOutOfRangeException(nameof(zodiacSign), zodiacSign, null)
        };
}