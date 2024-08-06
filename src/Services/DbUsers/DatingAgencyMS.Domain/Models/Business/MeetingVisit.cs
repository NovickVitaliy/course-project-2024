namespace DatingAgencyMS.Domain.Models.Business;

public class MeetingVisit
{
    public required int Id { get; init; }
    public required int ClientId { get; init; }
    public required int MeetingId { get; init; }
    public required bool Visited { get; init; }
}