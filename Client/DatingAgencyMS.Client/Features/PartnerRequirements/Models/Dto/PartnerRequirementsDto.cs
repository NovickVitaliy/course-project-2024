using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto;

public record PartnerRequirementsDto(
    int Id,
    string? Gender,
    string? Sex,
    int? MinAge,
    int? MaxAge,
    int? MinHeight,
    int? MaxHeight,
    int? MinWeight,
    int? MaxWeight,
    ZodiacSign? ZodiacSign,
    string? Location,
    int ClientId);