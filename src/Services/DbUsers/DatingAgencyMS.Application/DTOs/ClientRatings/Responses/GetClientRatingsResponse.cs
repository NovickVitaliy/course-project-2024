namespace DatingAgencyMS.Application.DTOs.ClientRatings.Responses;

public record GetClientRatingsResponse(ClientRatingDto[] ClientRatings, long Count);