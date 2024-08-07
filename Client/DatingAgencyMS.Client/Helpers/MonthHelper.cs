namespace DatingAgencyMS.Client.Helpers;

public static class MonthHelper
{
    public static string GetUkrainianMonthName(int month)
    {
        return month switch
        {
            1 => "Січень",
            2 => "Лютий",
            3 => "Березень",
            4 => "Квітень",
            5 => "Травень",
            6 => "Червень",
            7 => "Липень",
            8 => "Серпень",
            9 => "Вересень",
            10 => "Жовтень",
            11 => "Листопад",
            12 => "Грудень",
            _ => throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.")
        };
    }
}
