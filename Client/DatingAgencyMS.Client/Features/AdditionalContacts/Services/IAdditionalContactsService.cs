using DatingAgencyMS.Client.Features.AdditionalContacts.Models.DTOs.Requests;
using DatingAgencyMS.Client.Features.AdditionalContacts.Models.DTOs.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.AdditionalContacts.Services;

public interface IAdditionalContactsService
{
    [Get("/additional-contacts")]
    Task<GetAdditionalContactsResponse> Get(GetAdditionalContactsRequest request, [Authorize] string token);
}