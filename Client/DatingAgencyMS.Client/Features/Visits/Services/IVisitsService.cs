using DatingAgencyMS.Client.Features.Visits.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Visits.Models.Dtos.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.Visits.Services;

public interface IVisitsService
{
    [Get("/visits")]
    Task<GetVisitsResponse> GetVisits([Query] GetVisitsRequest request, [Authorize] string token);
}