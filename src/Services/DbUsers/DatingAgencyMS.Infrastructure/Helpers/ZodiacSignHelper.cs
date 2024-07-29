using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Infrastructure.Helpers;

public static class ZodiacSignHelper
{
    public static ZodiacSign FromUkrainianToZodiacSign(string ukrainianZodiacSign) =>
        ukrainianZodiacSign switch
        {
            "Овен" => ZodiacSign.Aries,
            "Телець" => ZodiacSign.Taurus,
            "Близнюки" => ZodiacSign.Gemini,
            "Рак" => ZodiacSign.Cancer,
            "Лев" => ZodiacSign.Leo,
            "Діва" => ZodiacSign.Virgo,
            "Терези" => ZodiacSign.Libra,
            "Скорпіон" => ZodiacSign.Scorpio,
            "Стрілець" => ZodiacSign.Sagittarius,
            "Козеріг" => ZodiacSign.Capricorn,
            "Водолій" => ZodiacSign.Aquarius,
            "Риби" => ZodiacSign.Pisces,
            _ => throw new ArgumentOutOfRangeException(nameof(ukrainianZodiacSign), ukrainianZodiacSign,
                "Invalid zodiac sign name")
        };

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