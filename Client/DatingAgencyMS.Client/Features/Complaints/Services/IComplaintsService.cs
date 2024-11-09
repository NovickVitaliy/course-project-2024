using DatingAgencyMS.Client.Features.Complaints.Models;
using DatingAgencyMS.Client.Features.Complaints.Models.Requests;
using DatingAgencyMS.Client.Features.Complaints.Models.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.Complaints.Services;

public interface IComplaintsService
{
    [Get("/complaints")]
    Task<GetComplaintsResponse> GetAsync(GetComplaintsRequest request, [Authorize] string token);

    [Get("/complaints/{id}")]
    Task<ComplaintDto> GetByIdAsync(int id, [Authorize] string token);

    [Post("/complaints")]
    Task CreateAsync(CreateComplaintRequest request, [Authorize] string token);

    [Put("/complaints/{id}")]
    Task UpdateAsync(int id, UpdateComplaintRequest request, [Authorize] string token);

    [Delete("/complaints/{id}")]
    Task DeleteAsync(int id, [Authorize] string token);
}