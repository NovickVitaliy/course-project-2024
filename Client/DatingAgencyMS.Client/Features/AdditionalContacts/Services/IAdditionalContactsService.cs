using DatingAgencyMS.Client.Features.AdditionalContacts.Models.DTOs.Requests;
using DatingAgencyMS.Client.Features.AdditionalContacts.Models.DTOs.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.AdditionalContacts.Services;

public interface IAdditionalContactsService
{
    [Get("/additional-contacts")]
    Task<GetAdditionalContactsResponse> GetAsync([Query] GetAdditionalContactsRequest request, [Authorize] string token);

    [Post("/additional-contacts")]
    Task CreateAsync([Body] CreateAdditionalContactsRequest request, [Authorize] string token);

    [Put("/additional-contacts/{id}")]
    Task UpdateAsync(int id, UpdateAdditionalContactsRequest request, [Authorize] string token);

    [Get("/additional-contacts/{id}")]
    Task<GetAdditionalContactsByIdResponse> GetByIdAsync(int id, [Authorize] string token);
}