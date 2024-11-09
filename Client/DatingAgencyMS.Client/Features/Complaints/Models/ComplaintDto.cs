using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Features.Complaints.Models;

public record ComplaintDto(
    int ComplaintId,
    int ComplainantId,
    int ComplaineeId,
    DateTime Date,
    string Text,
    ComplaintStatus ComplaintStatus);