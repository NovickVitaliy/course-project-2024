using DatingAgencyMS.Application.DTOs.PartnerRequirements.Requests;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IPartnerRequirementsService
{
    Task<ServiceResult<bool>> CreatePartnerRequirements(CreatePartnerRequirementsRequest request);
}