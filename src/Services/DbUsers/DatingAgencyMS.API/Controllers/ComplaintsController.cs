using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Complaints.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;

[Authorize]
[Route("api/complaints")]
public class ComplaintsController : BaseApiController
{
    private readonly IComplaintsService _complaintsService;
 
    public ComplaintsController(IComplaintsService complaintsService)
    {
        _complaintsService = complaintsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] GetComplaintsRequest request)
    {
        var result = await _complaintsService.GetComplaintsAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _complaintsService.GetComplaintByIdAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPost]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> CreateAsync(CreateComplaintRequest request)
    {
        var result = await _complaintsService.CreateComplaintAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Created($"/api/complaints/{result.ResponseData}", result.ResponseData);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> UpdateAsync(int id, UpdateComplaintRequest request)
    {
        request = request with { ComplaintId = id };
        var result = await _complaintsService.UpdateComplaintAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _complaintsService.DeleteComplaintAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }
}