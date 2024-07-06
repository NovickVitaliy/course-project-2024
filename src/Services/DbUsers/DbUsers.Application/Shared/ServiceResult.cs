namespace DbUsers.Application.Shared;

public record ServiceResult<TResponseData>(bool Success, int Code, TResponseData ResponseData, string Description = "");
