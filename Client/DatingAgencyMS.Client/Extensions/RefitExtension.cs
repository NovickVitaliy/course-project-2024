using System.Text.Json;
using DatingAgencyMS.Client.Models.DTOs.Common;
using Refit;

namespace DatingAgencyMS.Client.Extensions;

public static class RefitExtension
{
    public static ApiError ToApiError(this ApiException apiException)
    {
        return JsonSerializer.Deserialize<ApiError>(apiException.Content ?? "", new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        }) ?? new ApiError(400, "Api error occured");
    }
}