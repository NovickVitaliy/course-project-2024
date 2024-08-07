using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Meetings.Requests;
using DatingAgencyMS.Application.Extensions;

namespace DatingAgencyMS.API.Controllers;

[Route("api/meetings")]
public class MeetingsController : BaseApiController
{
    private readonly IMeetingsService _meetingsService;

    public MeetingsController(IMeetingsService meetingsService)
    {
        _meetingsService = meetingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMeetings([FromQuery] GetMeetingsRequest request)
    {
        var result = await _meetingsService.GetMeetings(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }
}