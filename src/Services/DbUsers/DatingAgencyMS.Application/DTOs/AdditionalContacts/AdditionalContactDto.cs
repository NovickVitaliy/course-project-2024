namespace DatingAgencyMS.Application.DTOs.AdditionalContacts;

public record AdditionalContactDto(
    int Id,
    int ClientId,
    string? Telegram,
    string? Facebook,
    string? Instagram,
    string? TikTok);