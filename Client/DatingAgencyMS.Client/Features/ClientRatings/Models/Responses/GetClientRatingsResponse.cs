namespace DatingAgencyMS.Client.Features.ClientRatings.Models.Responses;

public record GetClientRatingsResponse(ClientRatingDto[] ClientRatings, long Count);