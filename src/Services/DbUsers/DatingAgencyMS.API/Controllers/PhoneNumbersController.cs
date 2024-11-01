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
}