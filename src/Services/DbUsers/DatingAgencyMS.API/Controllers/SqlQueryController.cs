using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.SqlQueries;
using DatingAgencyMS.Application.Extensions;

namespace DatingAgencyMS.API.Controllers;

[Route("api/sql-queries")]
public class SqlQueryController : BaseApiController
{
    private readonly ISqlQueryService _sqlQueryService;
    
    public SqlQueryController(ISqlQueryService sqlQueryService)
    {
        _sqlQueryService = sqlQueryService;
    }
    
    [HttpPost]
    public async Task<IActionResult> RunAsync(SqlQueryRequest request)
    {
        var result = await _sqlQueryService.RunSqlAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.Code, result.ToHttpErrorResponse());
        }

        return Ok(result.ResponseData);
    }
}