namespace DatingAgencyMS.Client.Features.CoupleArchive.Models.DTOs.Responses;

public record GetArchivedCouplesResponse(IReadOnlyList<ArchivedCoupleDto> ArchivedCouples, long TotalCount);