namespace DatingAgencyMS.Domain.Models.Business;

public class Complaint
{
    public int ComplaintId { get; init; }
    public int ComplainantId  { get; init; }
    public int ComplaineeId { get; init; }
    public DateTime Date { get; init; }
    public string Text { get; init; }
    public ComplaintStatus ComplaintStatus { get; set; }
}