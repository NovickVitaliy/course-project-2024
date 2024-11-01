namespace DatingAgencyMS.Application.DTOs.PhoneNumbers.Responses;

public record GetPhoneNumbersResponse(PhoneNumberDto[] PhoneNumbers, long Count);