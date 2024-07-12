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

    public static IServiceCollection AddRefitServiceWithBaseApiUrl<TService>(this IServiceCollection services, IConfiguration cfg)
        where TService : class
    {
        services.AddRefitClient<TService>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(cfg["ApiBaseUrl"] ??
                                             throw new ArgumentException(
                                                 "Api base url was not found in configuration",
                                                 "ApiBaseUrl"));
            });
        return services;
    }
}