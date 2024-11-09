namespace DatingAgencyMS.Client.Features.Complaints.Models.Responses;

public record GetComplaintsResponse(ComplaintDto[] Complaints, long Count);