using DatingAgencyMS.Application.DTOs.PartnerRequirements.Requests;
using DatingAgencyMS.Application.DTOs.PartnerRequirements.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IPartnerRequirementsService
{
    Task<ServiceResult<bool>> CreatePartnerRequirements(CreatePartnerRequirementsRequest request);
    Task<ServiceResult<GetPartnersRequirementResponse>> GetPartnersRequirement(GetPartnersRequirementRequest request);
}