namespace DatingAgencyMS.Client.Models.Core;

public enum ComplaintStatus
{
    Pending,
    Approved,
    Denied
}

public static class ComplaintStatusTranslator
{
    public static string ToUkrainian(this ComplaintStatus complaintStatus)
        => complaintStatus switch
        {
            ComplaintStatus.Pending => "Очікується",
            ComplaintStatus.Approved => "Прийнятий",
            ComplaintStatus.Denied => "Відхилений",
            _ => throw new ArgumentOutOfRangeException(nameof(complaintStatus), complaintStatus, null)
        };

    public static ComplaintStatus ToEnum(string ukrainianValue)
        => ukrainianValue switch
        {
            "Очікується" => ComplaintStatus.Pending,
            "Прийнятий" => ComplaintStatus.Approved,
            "Відхилений" => ComplaintStatus.Denied,
            _ => throw new ArgumentOutOfRangeException(nameof(ukrainianValue), ukrainianValue, null)
        };
}