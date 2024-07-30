using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.PartnerRequirements.Requests;
using DatingAgencyMS.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;


[Authorize]
[Route("api/partner-requirements")]
public class PartnerRequirementsController : BaseApiController
{
    private readonly IPartnerRequirementsService _partnerRequirementsService;

    public PartnerRequirementsController(IPartnerRequirementsService partnerRequirementsService)
    {
        _partnerRequirementsService = partnerRequirementsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePartnerRequirements(CreatePartnerRequirementsRequest request)
    {
        var result = await _partnerRequirementsService.CreatePartnerRequirements(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Created();
    }
}