namespace DatingAgencyMS.Client.Features.ClientRatings.Models;

public record ClientRatingDto(
    int Id,
    int ClientId,
    int Rating,
    string Comment,
    DateTime RatingDate);