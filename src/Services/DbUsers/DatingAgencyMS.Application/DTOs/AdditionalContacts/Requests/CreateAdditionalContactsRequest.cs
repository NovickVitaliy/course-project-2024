namespace DatingAgencyMS.Application.DTOs.AdditionalContacts.Requests;

public record CreateAdditionalContactsRequest(
    int ClientId,
    string? Telegram,
    string? Facebook,
    string? Instagram,
    string? TikTok);