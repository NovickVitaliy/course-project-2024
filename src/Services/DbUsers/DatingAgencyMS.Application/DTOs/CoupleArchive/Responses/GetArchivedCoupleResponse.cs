namespace DatingAgencyMS.Application.DTOs.CoupleArchive.Responses;

public record GetArchivedCoupleResponse(IReadOnlyList<ArchivedCoupleDto> ArchivedCouples, long TotalCount);