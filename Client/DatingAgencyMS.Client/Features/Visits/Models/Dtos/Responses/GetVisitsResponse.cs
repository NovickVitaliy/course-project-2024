namespace DatingAgencyMS.Client.Features.Visits.Models.Dtos.Responses;

public record GetVisitsResponse(IReadOnlyList<VisitDto> Visits, long TotalCount);