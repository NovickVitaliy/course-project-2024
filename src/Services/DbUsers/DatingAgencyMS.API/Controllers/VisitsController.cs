using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Visits.Requests;
using DatingAgencyMS.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;

[Authorize]
[Route("api/visits")]
public class VisitsController : BaseApiController
{
    private readonly IVisitsService _visitsService;

    public VisitsController(IVisitsService visitsService)
    {
        _visitsService = visitsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVisits([FromQuery] GetVisitsRequest request)
    {
        var result = await _visitsService.GetVisits(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }
}