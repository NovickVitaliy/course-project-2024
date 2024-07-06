using DbUsers.API.Controllers.Base;
using DbUsers.Application.Contracts;
using DbUsers.Application.DTOs;
using DbUsers.Application.Extensions;

namespace DbUsers.API.Controllers;

public class DbAccessController : BaseApiController
{
    private readonly IDbManager _dbManager;

    public DbAccessController(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDbRequest request)
    {
        var result = await _dbManager.TryAccessDb(request.Login, request.Password);

        if (!result.Success)
        {
            return BadRequest(result.ToHttpErrorResponse());
        }
        
        //TODO: create jwt token with claim of user login
        
        return Ok();
    }

    [HttpPost("{login}")]
    public async Task<IActionResult> Connection([FromRoute]string login)
    {
        var result = await _dbManager.GetConnection(login);

        if (!result.Success)
        {
            return BadRequest(result.ToHttpErrorResponse());
        }

        return Ok();
    }

    [HttpPost("{login}")]
    public async Task<IActionResult> Close([FromRoute]string login)
    {
        var result = await _dbManager.CloseConnection(login);

        if (!result.Success)
        {
            return BadRequest(result.ToHttpErrorResponse());
        }

        return Ok();
    }
}