using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.CoupleArchive.Requests;
using DatingAgencyMS.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;

[Authorize]
[Route("api/archived-couples")]
public class CoupleArchiveController : BaseApiController
{
    private readonly ICoupleArchiveService _coupleArchiveService;

    public CoupleArchiveController(ICoupleArchiveService coupleArchiveService)
    {
        _coupleArchiveService = coupleArchiveService;
    }

    [HttpGet]
    public async Task<IActionResult> GetArchivedCouples([FromQuery] GetArchivedCoupleRequest request)
    {
        var result = await _coupleArchiveService.GetArchivedCouples(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetArchivedCouplesCount()
    {
        var result = await _coupleArchiveService.GetArchivedCouplesCount();
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToString());
        }

        return Ok(result.ResponseData);
    }
}