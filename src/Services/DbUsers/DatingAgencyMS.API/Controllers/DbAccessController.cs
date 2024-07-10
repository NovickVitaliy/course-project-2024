using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs;
using DatingAgencyMS.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DatingAgencyMS.API.Controllers;

public class DbAccessController : BaseApiController
{
    private readonly IDbManager _dbManager;
    private readonly ITokenService _tokenService;

    public DbAccessController(IDbManager dbManager, ITokenService tokenService)
    {
        _dbManager = dbManager;
        _tokenService = tokenService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDbRequest request)
    {
        var result = await _dbManager.TryAccessDb(request.Login, request.Password);

        if (!result.Success)
        {
            return BadRequest(result.ToHttpErrorResponse());
        }

        var token = await _tokenService.GenerateJwtToken(request.Login);

        return Ok(new { token });
    }

    [Authorize]
    [HttpPost("{login}")]
    public async Task<IActionResult> Connection([FromRoute] string login)
    {
        var result = await _dbManager.GetConnection(login);

        if (!result.Success)
        {
            return BadRequest(result.ToHttpErrorResponse());
        }

        return Ok();
    }
    
    [Authorize]
    [HttpPost("{login}")]
    public async Task<IActionResult> Close([FromRoute] string login)
    {
        var user = HttpContext.User.Claims;
        var result = await _dbManager.CloseConnection(login);

        if (!result.Success)
        {
            return BadRequest(result.ToHttpErrorResponse());
        }

        return Ok();
    }
}