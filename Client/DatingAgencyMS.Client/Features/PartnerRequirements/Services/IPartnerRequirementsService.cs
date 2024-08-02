using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Services;

public interface IPartnerRequirementsService
{
    [Post("/partner-requirements")]
    Task CreatePartnerRequirements(CreatePartnerRequirementsRequest request, [Authorize] string token);

    [Get("/partner-requirements")]
    Task<GetPartnerRequirementResponse> GetPartnerRequirements([Query]GetPartnersRequirementRequest request,
        [Authorize] string token);

    [Get("/partner-requirements/{partnerRequirementId}")]
    Task<GetPartnerRequirementByIdResponse> GetPartnerRequirementById(int partnerRequirementId, [Authorize] string token);

    [Put("/partner-requirements/{id}")]
    Task UpdatePartnerRequirements(int id, UpdatePartnerRequirementRequest request, [Authorize] string token);

    [Delete("/partner-requirements/{id}")]
    Task DeletePartnerRequirements(int id, [Authorize] string token);

    [Get("/partner-requirements/{id}/matches/count")]
    Task<long> GetMatchesCount(int id, [Authorize] string token);
}