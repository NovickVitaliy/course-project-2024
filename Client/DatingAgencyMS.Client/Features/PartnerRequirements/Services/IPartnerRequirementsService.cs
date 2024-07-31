using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Services;

public interface IPartnerRequirementsService
{
    [Post("/partner-requirements")]
    Task CreatePartnerRequirements(CreatePartnerRequirementsRequest request, [Authorize] string token);

    [Get("/partner-requirements")]
    Task<GetPartnerRequirementResponse> GetPartnerRequirements([Query]GetPartnersRequirementRequest request,
        [Authorize] string token);
}