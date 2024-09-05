namespace DatingAgencyMS.Client.Features.AdditionalContacts.Models;

public record AdditionalContactDto(
        int Id,
        int ClientId,
        string? Telegram,
        string? Facebook,
        string? Instagram,
        string? TikTok
    );