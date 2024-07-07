using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Extensions;

public static class ServiceResultExtensions
{
    public static object ToHttpErrorResponse<T>(this ServiceResult<T> serviceResult) => new
    {
        code = serviceResult.Code,
        description = serviceResult.Description
    };
}