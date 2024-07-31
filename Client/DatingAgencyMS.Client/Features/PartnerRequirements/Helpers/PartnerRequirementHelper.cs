using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Helpers;

public static class PartnerRequirementHelper
{
    public static UpdatePartnerRequirementRequest ToUpdateRequest(this PartnerRequirementsDto partnerRequirementsDto)
    {
        return new UpdatePartnerRequirementRequest()
        {
            City = partnerRequirementsDto.Location,
            Gender = partnerRequirementsDto.Gender,
            Sex = partnerRequirementsDto.Sex,
            ClientId = partnerRequirementsDto.ClientId,
            MaxAge = partnerRequirementsDto.MaxAge,
            ZodiacSign = partnerRequirementsDto.ZodiacSign,
            MaxHeight = partnerRequirementsDto.MaxHeight,
            MaxWeight = partnerRequirementsDto.MaxWeight,
            MinAge = partnerRequirementsDto.MinAge,
            MinHeight = partnerRequirementsDto.MinHeight,
            MinWeight = partnerRequirementsDto.MinWeight,
        };
    }
}