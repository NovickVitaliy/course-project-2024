namespace DbUsers.API.Models.ServiceResults;

public record ServiceResult<TResponseData>(bool Success, int Code, TResponseData ResponseData, string Description = "");
