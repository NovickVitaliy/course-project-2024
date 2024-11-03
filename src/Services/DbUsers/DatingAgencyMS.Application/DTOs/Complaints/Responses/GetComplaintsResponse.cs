namespace DatingAgencyMS.Application.DTOs.Complaints.Responses;

public record GetComplaintsResponse(ComplaintDto[] Complaints, long Count);