using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;


[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    private readonly IUserManager _userManager;

    public UsersController(IUserManager userManager)
    {
        _userManager = userManager;
    }
    
    [Authorize(Policy = ApplicationPolicies.IsOwnerOrAdmin)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _userManager.CreateUser(request);

        return Ok(new {result.Code, result.Description});
    }
}