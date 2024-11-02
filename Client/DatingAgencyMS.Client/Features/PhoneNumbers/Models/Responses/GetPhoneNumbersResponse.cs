namespace DatingAgencyMS.Client.Features.PhoneNumbers.Models.Responses;

public record GetPhoneNumbersResponse(PhoneNumberDto[] PhoneNumbers, long Count);