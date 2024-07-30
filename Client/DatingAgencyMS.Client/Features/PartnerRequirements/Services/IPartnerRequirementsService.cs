using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Services;

public interface IPartnerRequirementsService
{
    [Post("/partner-requirements")]
    Task CreatePartnerRequirements(CreatePartnerRequirementsRequest request, [Authorize] string token);
}