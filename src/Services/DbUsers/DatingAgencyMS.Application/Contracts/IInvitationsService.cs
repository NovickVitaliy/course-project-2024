using DatingAgencyMS.Application.DTOs.Invitations;
using DatingAgencyMS.Application.DTOs.Invitations.Requests;
using DatingAgencyMS.Application.DTOs.Invitations.Response;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IInvitationsService
{
    Task<ServiceResult<GetInvitationsResponse>> GetInvitations(GetInvitationsRequest request);
    Task<ServiceResult<int>> CreateInvitation(CreateInvitationRequest request);
    Task<ServiceResult<bool>> DeleteInvitation(int invitationId, string requestedBy);
    Task<ServiceResult<InvitationDto>> MarkAsAccepted(int invitationId);
}