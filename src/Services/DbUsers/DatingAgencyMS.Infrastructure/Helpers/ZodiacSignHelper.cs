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
        _ => throw new ArgumentOutOfRangeException(nameof(ukrainianZodiacSign), ukrainianZodiacSign, "Invalid zodiac sign name")
    };
}