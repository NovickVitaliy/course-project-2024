using System.Transactions;
using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Invitations.Requests;
using DatingAgencyMS.Application.DTOs.Meetings.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers; 

[Authorize]
[Route("api/invitations")]
public class InvitationsController : BaseApiController
{
    private readonly IInvitationsService _invitationsService;
    private readonly IMeetingsService _meetingsService;
    public InvitationsController(IInvitationsService invitationsService, IMeetingsService meetingsService)
    {
        _invitationsService = invitationsService;
        _meetingsService = meetingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvitations([FromQuery] GetInvitationsRequest request)
    {
        var result = await _invitationsService.GetInvitations(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPost]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> CreateInvitation(CreateInvitationRequest request)
    {
        var result = await _invitationsService.CreateInvitation(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Created("/api/invitation/{id}", new {id = result.ResponseData});
    }

    [HttpDelete("{invitationId:int}")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> DeleteInvitation(int invitationId)
    {
        var requestedBy = User.GetDbUserLogin();
        var result = await _invitationsService.DeleteInvitation(invitationId, requestedBy);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpPut("{invitationId:int}/accept")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> MarkAsAccepted(int invitationId)
    {
        try
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var invitationResult = await _invitationsService.MarkAsAccepted(invitationId);
            if (!invitationResult.Success)
            {
                return StatusCode(invitationResult.Code, invitationResult.ToHttpErrorResponse());
            }

            var invitationDto = invitationResult.ResponseData;
            var createMeetingRequest = new CreateMeetingRequest(invitationDto.InviterId, invitationDto.InviteeId,
                invitationDto.Location, invitationDto.DateOfMeeting);
            var meetingResult = await _meetingsService.CreateMeeting(createMeetingRequest);
            if (!meetingResult.Success)
            {
                return StatusCode(meetingResult.Code, meetingResult.ToHttpErrorResponse());
            }
            transactionScope.Complete();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }
}