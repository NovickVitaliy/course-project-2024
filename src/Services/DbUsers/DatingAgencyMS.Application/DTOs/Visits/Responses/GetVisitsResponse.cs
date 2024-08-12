namespace DatingAgencyMS.Application.DTOs.Visits.Responses;

public record GetVisitsResponse(IReadOnlyList<VisitDto> Visits, long TotalCount);