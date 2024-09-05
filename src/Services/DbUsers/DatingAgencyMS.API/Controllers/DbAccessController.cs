using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs;
using DatingAgencyMS.Application.DTOs.DbAccess;
using DatingAgencyMS.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;

[Route("api/dbaccess")]
public class DbAccessController : BaseApiController
{
    private readonly IDbManager _dbManager;
    private readonly ITokenService _tokenService;
    private readonly IUserManager _userManager;

    public DbAccessController(IDbManager dbManager, ITokenService tokenService, IUserManager userManager)
    {
        _dbManager = dbManager;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDbRequest request)
    {
        var credentialsCheck = await _userManager.CheckUserCredentials(request);
        if (!credentialsCheck.Success)
        {
            return StatusCode(credentialsCheck.Code, credentialsCheck.ToHttpErrorResponse());
        }
        
        var result = await _dbManager.TryAccessDb(request.Login, request.Password);

        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }
        
        var role = (await _userManager.GetUserRole(request.Login)).ResponseData;
        var token = await _tokenService.GenerateJwtToken(request.Login, role);
        return Ok(new { login = request.Login, token, role });
    }

    [Authorize]
    [HttpGet("{login}/connection")]
    public async Task<IActionResult> Connection([FromRoute] string login)
    {
        var result = await _dbManager.GetConnection(login);

        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok();
    }

    [Authorize]
    [HttpPost("{login}/close")]
    public async Task<IActionResult> Close([FromRoute] string login)
    {
        var user = HttpContext.User.Claims;
        var result = await _dbManager.CloseConnection(login);

        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var response = await _userManager.GetForgottenPassword(request);
        if (!response.Success)
        {
            return StatusCode(response.Code, response.ToHttpErrorResponse());
        }

        return Ok(response.ResponseData);
    }
}