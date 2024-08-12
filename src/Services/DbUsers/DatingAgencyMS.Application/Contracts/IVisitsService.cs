using DatingAgencyMS.Application.DTOs.Visits.Requests;
using DatingAgencyMS.Application.DTOs.Visits.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IVisitsService
{
    Task<ServiceResult<GetVisitsResponse>> GetVisits(GetVisitsRequest request);
}