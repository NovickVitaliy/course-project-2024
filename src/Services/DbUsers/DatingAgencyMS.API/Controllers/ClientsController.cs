using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Clients.Requests;
using DatingAgencyMS.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ClientsController : BaseApiController
{
    private readonly IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetClients([FromQuery] GetClientsRequest request)
    {
        var result = await _clientsService.GetClients(request);

        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }
        
        return Ok(result.ResponseData);
    }
}