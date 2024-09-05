namespace DatingAgencyMS.Application.DTOs.AdditionalContacts.Requests;

public record UpdateAdditionalContactsRequest(
    int Id,
    string? Telegram,
    string? Facebook,
    string? Instagram,
    string? TikTok);