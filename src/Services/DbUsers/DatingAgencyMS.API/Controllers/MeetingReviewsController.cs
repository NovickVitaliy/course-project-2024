using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.MeetingReviews.Requests;
using DatingAgencyMS.Application.Extensions;

namespace DatingAgencyMS.API.Controllers;

[Route("api/meeting-reviews")]
public class MeetingReviewsController : BaseApiController
{
    private readonly IMeetingReviewsService _meetingReviewsService;
    
    public MeetingReviewsController(IMeetingReviewsService meetingReviewsService)
    {
        _meetingReviewsService = meetingReviewsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] GetMeetingReviewsRequest request)
    {
        var result = await _meetingReviewsService.GetMeetingReviewsAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _meetingReviewsService.GetMeetingReviewByIdAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateMeetingReviewRequest request)
    {
        var result = await _meetingReviewsService.CreateMeetingReviewAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Created($"/api/meeting-reviews/{result.ResponseData}", result.ResponseData);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateMeetingReviewRequest request)
    {
        request = request with { Id = id};
        var result = await _meetingReviewsService.UpdateMeetingReviewAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _meetingReviewsService.DeleteMeetingReviewAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }
}