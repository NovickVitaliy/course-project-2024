namespace DatingAgencyMS.Client.Features.Visits.Models.Dtos;

public record VisitDto(
    int Id,
    int ClientId,
    int MeetingId,
    bool Visited);