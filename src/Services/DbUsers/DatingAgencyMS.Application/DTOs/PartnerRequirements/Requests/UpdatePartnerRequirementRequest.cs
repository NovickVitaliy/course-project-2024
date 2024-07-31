using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Application.DTOs.PartnerRequirements.Requests;

public record UpdatePartnerRequirementRequest(string? Gender,
    string? Sex,
    int? MinAge,
    int? MaxAge,
    int? MinHeight,
    int? MaxHeight,
    int? MinWeight,
    int? MaxWeight,
    ZodiacSign? ZodiacSign,
    string? City,
    int ClientId,
    string RequestedBy);