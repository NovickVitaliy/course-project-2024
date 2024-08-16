using System.Transactions;
using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Meetings.Requests;
using DatingAgencyMS.Application.Extensions;

namespace DatingAgencyMS.API.Controllers;

[Route("api/meetings")]
public class MeetingsController : BaseApiController
{
    private readonly IMeetingsService _meetingsService;
    private readonly ICoupleArchiveService _coupleArchiveService;
        
    public MeetingsController(IMeetingsService meetingsService, ICoupleArchiveService coupleArchiveService)
    {
        _meetingsService = meetingsService;
        _coupleArchiveService = coupleArchiveService;
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
        try
        {
            request = request with { MeetingId = meetingId };
            //TODO: transaction
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            var result = await _meetingsService.ChangeMeetingStatus(request);
            if (!result.Success)
            {
                return StatusCode(result.Code, result.ToHttpErrorResponse());
            }

            result = await _coupleArchiveService.MoveCoupleToArchive(request.MeetingId);
            if (!result.Success)
            {
                return StatusCode(result.Code, result.ToHttpErrorResponse());
            }
            
            transactionScope.Complete();
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}