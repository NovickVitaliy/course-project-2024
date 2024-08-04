using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.PartnerRequirements.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Extensions;
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
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> CreatePartnerRequirements(CreatePartnerRequirementsRequest request)
    {
        var result = await _partnerRequirementsService.CreatePartnerRequirements(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetPartnerRequirements([FromQuery] GetPartnersRequirementRequest request)
    {
        var result = await _partnerRequirementsService.GetPartnersRequirement(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("{partnerRequirementId:int}")]
    public async Task<IActionResult> GetPartnerRequirementId(int partnerRequirementId)
    {
        var requestedBy = User.GetDbUserLogin();
        var result = await _partnerRequirementsService.GetPartnerRequirementById(partnerRequirementId, requestedBy);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPut("{partnerRequirementId:int}")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> UpdatePartnerRequirement([FromRoute] int partnerRequirementId,
        [FromBody] UpdatePartnerRequirementRequest request)
    {
        var result = await _partnerRequirementsService.UpdatePartnerRequirement(partnerRequirementId, request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> DeletePartnerRequirements(int id)
    {
        var requestedBy = User.GetDbUserLogin();
        var result = await _partnerRequirementsService.DeletePartnerRequirements(id, requestedBy);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpGet("{id:int}/matches/count")]
    public async Task<IActionResult> GetMatchesCount(int id)
    {
        var requestedBy = User.GetDbUserLogin();
        var result = await _partnerRequirementsService.GetMatchesCount(id, requestedBy);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }
}