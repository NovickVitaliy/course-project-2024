using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.UserManagement.Requests;
using DatingAgencyMS.Application.Extensions;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

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
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }
        return Ok(new {result.Code, result.Description});
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request)
    {
        var result = await _userManager.GetUsers(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpGet("{login}")]
    public async Task<IActionResult> GetUser(string login)
    {
        var requestedBy = User.GetDbUserLogin();
        var request = new GetUserRequest(login, requestedBy);
        var result = await _userManager.GetUser(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromQuery] DeleteUserRequest request)
    {
        var result = await _userManager.DeleteUser(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }

    [HttpPut("{userId:int}")]
    public async Task<IActionResult> AssignNewRole([FromRoute] int _, [FromBody] AssignNewRoleRequest request)
    {
        var result = await _userManager.AssignNewRole(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok();
    } 
}