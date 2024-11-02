namespace DatingAgencyMS.Application.DTOs.ClientRatings;

public record ClientRatingDto(
    int Id,
    int ClientId,
    int Rating,
    string Comment,
    DateTime RatingDate);