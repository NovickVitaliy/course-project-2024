namespace DatingAgencyMS.Application.DTOs.ClientRatings.Requests;

public record UpdateClientRatingRequest(
    int Id,
    int Rating,
    string Comment);