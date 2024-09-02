using DatingAgencyMS.Client.Features.CoupleArchive.Models.DTOs.Requests;
using DatingAgencyMS.Client.Features.CoupleArchive.Models.DTOs.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.CoupleArchive.Services;

public interface ICoupleArchiveService
{
    [Get("/archived-couples")]
    Task<GetArchivedCouplesResponse> GetArchivedCouples([Query] GetArchivedCoupleRequest request, [Authorize] string token);

    [Get("/archived-couples/count")]
    Task<long> GetArchivedCoupleCount([Authorize] string token);
}