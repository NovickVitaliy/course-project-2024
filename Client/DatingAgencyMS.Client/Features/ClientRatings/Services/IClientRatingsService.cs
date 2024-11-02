using DatingAgencyMS.Client.Features.ClientRatings.Models;
using DatingAgencyMS.Client.Features.ClientRatings.Models.Requests;
using DatingAgencyMS.Client.Features.ClientRatings.Models.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.ClientRatings.Services;

public interface IClientRatingsService
{
    [Get("/client-ratings")]
    Task<GetClientRatingsResponse> GetAsync(GetClientRatingsRequest request, [Authorize] string token);

    [Get("/client-ratings/{id}")]
    Task<ClientRatingDto> GetByIdAsync(int id, [Authorize] string token);

    [Post("/client-ratings")]
    Task CreateAsync(CreateClientRatingRequest request, [Authorize] string token);

    [Put("/client-ratings/{id}")]
    Task UpdateAsync(int id, UpdateClientRatingRequest request, [Authorize] string token);

    [Delete("/client-ratings/{id}")]
    Task DeleteAsync(int id, [Authorize] string token);
}