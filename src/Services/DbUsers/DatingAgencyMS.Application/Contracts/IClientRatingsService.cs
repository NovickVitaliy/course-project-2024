using DatingAgencyMS.Application.DTOs.ClientRatings;
using DatingAgencyMS.Application.DTOs.ClientRatings.Requests;
using DatingAgencyMS.Application.DTOs.ClientRatings.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IClientRatingsService
{
    Task<ServiceResult<int>> CreateClientRatingAsync(CreateClientRatingRequest request);
    Task<ServiceResult<GetClientRatingsResponse>> GetClientRatingsAsync(GetClientRatingsRequest request);
    Task<ServiceResult<ClientRatingDto>> GetClientRatingByIdAsync(int id);
    Task<ServiceResult<bool>> UpdateClientRatingAsync(UpdateClientRatingRequest request);
    Task<ServiceResult<bool>> DeleteClientRatingAsync(int id);
}