namespace DatingAgencyMS.Application.DTOs.Visits;

public record VisitDto(
    int Id,
    int ClientId,
    int MeetingId,
    bool Visited);