using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Application.DTOs.UserManagement.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Common;
using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Keys;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;


[Route("api/[controller]")]
[Authorize(Policy = ApplicationPolicies.IsOwnerOrAdmin)]
public class UsersController : BaseApiController
{
    private readonly IUserManager _userManager;

    public UsersController(IUserManager userManager)
    {
        _userManager = userManager;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _userManager.CreateUser(request);

        return Ok(new {result.Code, result.Description});
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request)
    {
        var serviceResult = await _userManager.GetUsers(request);
        if (!serviceResult.Success)
        {
            return BadRequest(serviceResult.ToHttpErrorResponse());
        }

        return Ok(serviceResult.ResponseData);
    }
}