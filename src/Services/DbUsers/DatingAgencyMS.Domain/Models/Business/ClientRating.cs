namespace DatingAgencyMS.Domain.Models.Business;

public class ClientRating
{
    public int RatingId { get; init; }
    public int ClientId { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime RatingDate { get; init; }
}