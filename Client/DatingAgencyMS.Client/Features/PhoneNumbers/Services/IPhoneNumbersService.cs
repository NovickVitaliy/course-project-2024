using DatingAgencyMS.Client.Features.PhoneNumbers.Models;
using DatingAgencyMS.Client.Features.PhoneNumbers.Models.Requests;
using DatingAgencyMS.Client.Features.PhoneNumbers.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace DatingAgencyMS.Client.Features.PhoneNumbers.Services;

public interface IPhoneNumbersService
{
    [Get("/phone-numbers")]
    Task<GetPhoneNumbersResponse> GetAsync(GetPhoneNumbersRequest request, [Authorize] string token);

    [Get("/phone-numbers/{id}")]
    Task<PhoneNumberDto> GetByIdAsync(int id, [Authorize] string token);

    [Post("/phone-numbers")]
    Task CreateAsync(CreatePhoneNumberRequest request, [Authorize] string token);

    [Put("/phone-numbers/{id}")]
    Task UpdateAsync(int id, UpdatePhoneNumberRequest request, [Authorize] string token);

    [Delete("/phone-numbers/{id}")]
    Task DeleteAsync(int id, [Authorize] string token);
}