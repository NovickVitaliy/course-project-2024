using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Clients.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Infrastructure.Extensions;
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

    [HttpGet("{clientId:int}")]
    public async Task<IActionResult> GetClient(int clientId)
    {
        var requestedBy = User.GetDbUserLogin();
        var result = await _clientsService.GetClientById(new GetClientRequest(clientId, requestedBy));

        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
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

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest request)
    {
        var result = await _clientsService.CreateClient(request);

        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        //TODO: refactor return
        return Ok(result.ResponseData);
    }

    [HttpDelete("{clientId:int}")]
    public async Task<IActionResult> DeleteClient(int clientId)
    {
        var requestedBy = User.GetDbUserLogin();
        var result = await _clientsService.DeleteClient(clientId, requestedBy);

        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpPut("{clientId:int}")]
    public async Task<IActionResult> UpdateClient([FromRoute] int clientId, [FromBody] UpdateClientRequest request)
    {
        var result = await _clientsService.UpdateClient(clientId, request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }
}