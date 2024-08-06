using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Invitations.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Infrastructure.Extensions;

namespace DatingAgencyMS.API.Controllers; 

[Route("api/invitations")]
public class InvitationsController : BaseApiController
{
    private readonly IInvitationsService _invitationsService;

    public InvitationsController(IInvitationsService invitationsService)
    {
        _invitationsService = invitationsService;
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
    public async Task<IActionResult> MarkAssAccepted(int invitationId)
    {
        var result = await _invitationsService.MarkAsAccepted(invitationId);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok();
    }
}