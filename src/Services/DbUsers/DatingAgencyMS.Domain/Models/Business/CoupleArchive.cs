namespace DatingAgencyMS.Domain.Models.Business;

public class CoupleArchive
{
    public int CoupleArchiveId { get; init; }
    public int FirstClientId { get; init; }
    public int SecondClientId { get; init; }
    public DateOnly CoupleCreatedOn { get; init; }
    public string AdditionalInfo { get; init; } = string.Empty;
    public DateTime ArchivedOn { get; init; }
}