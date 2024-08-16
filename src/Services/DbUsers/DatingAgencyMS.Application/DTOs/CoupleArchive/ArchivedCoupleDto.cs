namespace DatingAgencyMS.Application.DTOs.CoupleArchive;

public record ArchivedCoupleDto(
    int CoupleArchiveId,
    int FirstClientId,
    int SecondClientId,
    DateOnly CoupleCreatedOn,
    string AdditionalInfo,
    DateTime ArchivedOn);