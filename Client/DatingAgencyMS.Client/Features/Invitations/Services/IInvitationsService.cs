using DatingAgencyMS.Client.Features.Invitations.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Invitations.Models.Dtos.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.Invitations.Services;

public interface IInvitationsService
{
    [Get("/invitations")]
    Task<GetinvitationsResponse> GetInvitations([Query] GetInvitationsRequest request, [Authorize] string token);

    [Post("/invitations")]
    Task CreateInvitation(CreateInvitationRequest request, [Authorize] string token);

    [Delete("/invitations/{id}")]
    Task DeleteInvitation(int id, [Authorize] string token);
}