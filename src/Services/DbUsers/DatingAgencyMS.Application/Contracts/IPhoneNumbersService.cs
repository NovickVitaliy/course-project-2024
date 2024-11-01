using DatingAgencyMS.Application.DTOs.PhoneNumbers;
using DatingAgencyMS.Application.DTOs.PhoneNumbers.Requests;
using DatingAgencyMS.Application.DTOs.PhoneNumbers.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IPhoneNumbersService
{
    Task<ServiceResult<int>> CreatePhoneNumberAsync(CreatePhoneNumberRequest request);
    Task<ServiceResult<PhoneNumberDto>> GetPhoneNumberByIdAsync(int id);
    Task<ServiceResult<GetPhoneNumbersResponse>> GetPhoneNumbersAsync(GetPhoneNumbersRequest request);
    Task<ServiceResult<bool>> UpdatePhoneNumberAsync(UpdatePhoneNumberRequest request);
    Task<ServiceResult<bool>> DeletePhoneNumberAsync(int id);
}