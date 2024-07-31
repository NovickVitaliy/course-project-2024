using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Application.DTOs.PartnerRequirements;

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