namespace DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Responses;

public record GetPartnerRequirementResponse(IReadOnlyList<PartnerRequirementsDto> PartnerRequirements, long Count);