using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.AdditionalContacts.Requests;
using DatingAgencyMS.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;

[Authorize]
[Route("api/additional-contacts")]
public class AdditionalContactsController : BaseApiController
{
    private readonly IAdditionalContactsService _additionContactsService;

    public AdditionalContactsController(IAdditionalContactsService additionContactsService)
    {
        _additionContactsService = additionContactsService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAdditionalContactsRequest request)
    {
        var result = await _additionContactsService.GetAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdditionalContactsRequest request)
    {
        var result = await _additionContactsService.CreateAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Created();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateAdditionalContactsRequest request)
    {
        var response = await _additionContactsService.UpdateAsync(request);
        if (!response.Success)
        {
            return StatusCode(response.Code, response.ToHttpErrorResponse());
        }

        return Ok();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _additionContactsService.GetByIdAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }
}