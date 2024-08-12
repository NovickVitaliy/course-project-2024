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

    [HttpGet("planned")]
    public async Task<IActionResult> GetPlannedMeetingsByPeriod([FromQuery] GetPlannedMeetingsForPeriodRequest request)
    {
        var result = await _meetingsService.GetPlannedMeetingsByPeriod(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPut("{meetingId:int}/change-status")]
    public async Task<IActionResult> ChangeMeetingStatus([FromBody] ChangeMeetingStatusRequest request,
        [FromRoute] int meetingId)
    { 
        request = request with { MeetingId = meetingId };

        var result = await _meetingsService.ChangeMeetingStatus(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok();
    }
}