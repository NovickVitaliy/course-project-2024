using DatingAgencyMS.Application.DTOs.Invitations.Requests;
using DatingAgencyMS.Application.DTOs.Invitations.Response;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IInvitationsService
{
    Task<ServiceResult<GetInvitationsResponse>> GetInvitations(GetInvitationsRequest request);
}