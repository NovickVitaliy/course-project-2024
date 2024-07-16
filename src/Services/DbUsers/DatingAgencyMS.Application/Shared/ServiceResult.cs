using System.Net;

namespace DatingAgencyMS.Application.Shared;

public record ServiceResult<TResponseData>(bool Success, int Code, TResponseData? ResponseData, string Description = "")
{
    public static ServiceResult<TResponseData> Ok(TResponseData responseData)
        => new ServiceResult<TResponseData>(true, (int)HttpStatusCode.OK, responseData);

    public static ServiceResult<TResponseData> BadRequest(string description)
        => new(false, (int)HttpStatusCode.BadRequest, default, description);

    public static ServiceResult<TResponseData> NotFound(string entityName, string identifier)
        => new(false, (int)HttpStatusCode.NotFound, default,
            $"Сутність '{entityName}' з ключем: {identifier} не була знайдена");

    public static ServiceResult<TResponseData> Conflict(string description)
        => new ServiceResult<TResponseData>(false, (int)HttpStatusCode.Conflict, default, description);

    public static ServiceResult<TResponseData> Created(TResponseData responseData)
        => new(true, (int)HttpStatusCode.Created, responseData);
}