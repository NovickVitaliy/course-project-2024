namespace DatingAgencyMS.Client.Features.CoupleArchive.Models.DTOs;

public record ArchivedCoupleDto(
    int CoupleArchiveId,
    int FirstClientId,
    int SecondClientId,
    DateOnly CoupleCreatedOn,
    string AdditionalInfo,
    DateTime ArchivedOn);