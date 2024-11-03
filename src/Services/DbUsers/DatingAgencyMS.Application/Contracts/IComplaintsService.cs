using DatingAgencyMS.Application.DTOs.Complaints;
using DatingAgencyMS.Application.DTOs.Complaints.Requests;
using DatingAgencyMS.Application.DTOs.Complaints.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IComplaintsService
{
    Task<ServiceResult<int>> CreateComplaintAsync(CreateComplaintRequest request);
    Task<ServiceResult<GetComplaintsResponse>> GetComplaintsAsync(GetComplaintsRequest request);
    Task<ServiceResult<ComplaintDto>> GetComplaintByIdAsync(int id);
    Task<ServiceResult<bool>> UpdateComplaintAsync(UpdateComplaintRequest request);
    Task<ServiceResult<bool>> DeleteComplaintAsync(int id);
}