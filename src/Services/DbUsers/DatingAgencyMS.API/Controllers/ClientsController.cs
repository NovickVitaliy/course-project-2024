using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Clients.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Client.Constants;
using DatingAgencyMS.Infrastructure.Constants;
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
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
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
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
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
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> UpdateClient([FromRoute] int clientId, [FromBody] UpdateClientRequest request)
    {
        var result = await _clientsService.UpdateClient(clientId, request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpGet("declined/count")]
    public async Task<IActionResult> GetCountOfClientsWhoDeclinedService()
    {
        var requestedBy = User.GetDbUserLogin();
        var result = await _clientsService.GetCountOfClientsWhoDeclinedService(requestedBy);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("by-year-quarter")]
    public async Task<IActionResult> GetClientsByYearQuarters([FromQuery] GetClientsByYearQuarterRequest request)
    {
        var result = await _clientsService.GetClientsByYearQuarter(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("registered")]
    public async Task<IActionResult> GetRegisteredClientByPeriod([FromQuery] GetClientsByTimePeriodRequest request)
    {
        var result = await _clientsService.GetRegisteredClientsByPeriod(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpDelete("declined")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> DeleteClientWhoDeclinedService()
    {
        var requestedBy = User.GetDbUserLogin();
        var result = await _clientsService.DeleteClientsWhoDeclinedService(requestedBy);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpGet("{clientId:int}/requirements/{requirementId:int}/matches")]
    public async Task<IActionResult> GetMatchingPartners(int clientId, int requirementId, 
        [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var requestedBy = User.GetDbUserLogin();
        var request = new GetMatchingPartnersRequest(clientId, requirementId, requestedBy, pageNumber, pageSize);
        var result = await _clientsService.GetMatchingPartners(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("not-skipped")]
    public async Task<IActionResult> GetClientWhoDidNotSkipAnyMeeting([FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        var result = await _clientsService.GetClientsWhoDidNotSkipAnyMeeting(pageNumber, pageSize);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }
}