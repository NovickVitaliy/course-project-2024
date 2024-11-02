using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.PhoneNumbers.Requests;
using DatingAgencyMS.Application.Extensions;

namespace DatingAgencyMS.API.Controllers;

[Route("api/phone-numbers")]
public class PhoneNumbersController : BaseApiController
{
    private readonly IPhoneNumbersService _phoneNumbersService;
    
    public PhoneNumbersController(IPhoneNumbersService phoneNumbersService)
    {
        _phoneNumbersService = phoneNumbersService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreatePhoneNumberRequest request)
    {
        var result = await _phoneNumbersService.CreatePhoneNumberAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Created($"api/phone-numbers/{result.ResponseData}", new {id = result.ResponseData});
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] GetPhoneNumbersRequest request)
    {
        var result = await _phoneNumbersService.GetPhoneNumbersAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _phoneNumbersService.GetPhoneNumberByIdAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdatePhoneNumberRequest request)
    {
        request = request with { Id = id };
        var result = await _phoneNumbersService.UpdatePhoneNumberAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _phoneNumbersService.DeletePhoneNumberAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return NoContent();
    }
}