using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Invitations.Requests;
using DatingAgencyMS.Application.Extensions;

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
}