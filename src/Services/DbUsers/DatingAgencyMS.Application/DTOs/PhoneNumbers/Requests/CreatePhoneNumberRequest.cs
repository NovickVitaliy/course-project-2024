using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Application.DTOs.PhoneNumbers.Requests;

public record CreatePhoneNumberRequest(
    string PhoneNumber,
    int AdditionalContactsId);