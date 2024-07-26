namespace DatingAgencyMS.Domain.Models.Business;

public class Friendship
{
    public int FirstClientId { get; init; }
    public int SecondClientId { get; init; }
    public DateTime EstablishedOn { get; init; }
}