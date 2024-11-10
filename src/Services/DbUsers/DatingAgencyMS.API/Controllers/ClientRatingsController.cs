using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.ClientRatings.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;

[Authorize]
[Route("api/client-ratings")]
public class ClientRatingsController : BaseApiController
{
    private readonly IClientRatingsService _clientRatingsService;
    public ClientRatingsController(IClientRatingsService clientRatingsService)
    {
        _clientRatingsService = clientRatingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] GetClientRatingsRequest request)
    {
        var result = await _clientRatingsService.GetClientRatingsAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _clientRatingsService.GetClientRatingByIdAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPost]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> CreateAsync(CreateClientRatingRequest request)
    {
        var result = await _clientRatingsService.CreateClientRatingAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Created($"/api/client-ratings/{result.ResponseData}", result.ResponseData);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> UpdateAsync(int id, UpdateClientRatingRequest request)
    {
        request = request with { Id = id };
        var result = await _clientRatingsService.UpdateClientRatingAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = ApplicationPolicies.CreateUpdateDeleteAccess)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _clientRatingsService.DeleteClientRatingAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }
}