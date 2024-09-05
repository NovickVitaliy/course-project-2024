using DatingAgencyMS.Application.DTOs.AdditionalContacts.Requests;
using DatingAgencyMS.Application.DTOs.AdditionalContacts.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IAdditionalContactsService
{
    Task<ServiceResult<GetAdditionalContactsResponse>> GetAsync(GetAdditionalContactsRequest request);
    Task<ServiceResult<bool>> CreateAsync(CreateAdditionalContactsRequest request);
    Task<ServiceResult<bool>> UpdateAsync(UpdateAdditionalContactsRequest request);
    Task<ServiceResult<GetAdditionalContactByIdResponse>> GetByIdAsync(int id);
}