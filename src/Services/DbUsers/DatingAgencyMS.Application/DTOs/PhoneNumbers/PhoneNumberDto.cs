namespace DatingAgencyMS.Application.DTOs.PhoneNumbers;

public record PhoneNumberDto(
    int Id,
    string PhoneNumber,
    int AdditionalContactsId);