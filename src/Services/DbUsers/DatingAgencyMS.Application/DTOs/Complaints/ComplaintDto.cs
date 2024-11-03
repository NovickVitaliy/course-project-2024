using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Application.DTOs.Complaints;

public record ComplaintDto(
    int ComplaintId,
    int ComplainantId,
    int ComplaineeId,
    DateTime Date,
    string Text,
    ComplaintStatus ComplaintStatus);