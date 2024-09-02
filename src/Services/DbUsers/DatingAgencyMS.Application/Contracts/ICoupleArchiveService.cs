using DatingAgencyMS.Application.DTOs.CoupleArchive.Requests;
using DatingAgencyMS.Application.DTOs.CoupleArchive.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface ICoupleArchiveService
{
    Task<ServiceResult<bool>> MoveCoupleToArchive(int successfulMeetingId);
    Task<ServiceResult<GetArchivedCoupleResponse>> GetArchivedCouples(GetArchivedCoupleRequest request);
    Task<ServiceResult<long>> GetArchivedCouplesCount();
}