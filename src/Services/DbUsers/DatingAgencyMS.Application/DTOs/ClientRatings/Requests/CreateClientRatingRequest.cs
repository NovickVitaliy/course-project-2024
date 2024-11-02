namespace DatingAgencyMS.Application.DTOs.ClientRatings.Requests;

public record CreateClientRatingRequest(
    int ClientId,
    int Rating,
    string Comment);