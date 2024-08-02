using DatingAgencyMS.Application.DTOs.PartnerRequirements.Requests;
using DatingAgencyMS.Application.DTOs.PartnerRequirements.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IPartnerRequirementsService
{
    Task<ServiceResult<bool>> CreatePartnerRequirements(CreatePartnerRequirementsRequest request);
    Task<ServiceResult<GetPartnersRequirementResponse>> GetPartnersRequirement(GetPartnersRequirementRequest request);
    Task<ServiceResult<GetPartnerRequirementResponse>> GetPartnerRequirementById(int id, string requestedBy);
    Task<ServiceResult<bool>> UpdatePartnerRequirement(int partnerRequirementId, UpdatePartnerRequirementRequest request);
    Task<ServiceResult<bool>> DeletePartnerRequirements(int id, string requestedBy);
    Task<ServiceResult<long>> GetMatchesCount(int id, string requestedBy);
}