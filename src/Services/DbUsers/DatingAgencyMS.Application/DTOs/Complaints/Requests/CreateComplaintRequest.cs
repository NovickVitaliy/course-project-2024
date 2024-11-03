namespace DatingAgencyMS.Application.DTOs.Complaints.Requests;

public record CreateComplaintRequest(
    int ComplainantId,
    int ComplaineeId,
    string Text);