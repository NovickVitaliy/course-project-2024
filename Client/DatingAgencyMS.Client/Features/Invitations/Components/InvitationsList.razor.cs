using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Invitations.Models.Dtos;
using DatingAgencyMS.Client.Features.Invitations.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Invitations.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Invitations.Components;

public partial class InvitationsList : ComponentBase
{
    [Inject] private IInvitationsService InvitationsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    
    private async Task<GridDataProviderResult<InvitationDto>> InvitationDataProvider(
        GridDataProviderRequest<InvitationDto> request)
    {
        var invitationRequest = BuildGetInvitationsRequest(request);
        try
        {
            var response = await InvitationsService.GetInvitations(invitationRequest, UserState.Value.User.Token);

            return new GridDataProviderResult<InvitationDto>()
            {
                Data = response.Invitations,
                TotalCount = (int?)response.TotalCount
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<InvitationDto>()
            {
                TotalCount = 0,
                Data = []
            };
        }
    }

    private GetInvitationsRequest BuildGetInvitationsRequest(GridDataProviderRequest<InvitationDto> request)
    {
        var invitationIdFilter = request.Filters.GetIntegerFilter("InvitationId");
        var inviterIdFilter = request.Filters.GetIntegerFilter("InviterId");
        var inviteeIdFilter = request.Filters.GetIntegerFilter("InviteeId");
        var locationFilter = request.Filters.GetStringFilter("Location");
        var dateOfMeetingFilter = request.Filters.GetDateTimeFilter("DateOfMeeting");
        var createdOnFilter = request.Filters.GetDateOnlyFilter("CreatedOn");
        var activeToFilter = request.Filters.GetDateOnlyFilter("ActiveTo");
        var isAcceptedFilter = request.Filters.GetBooleanFilter("IsAccepted");
        var sorting = request.Sorting.FirstOrDefault();
        SortingInfo? sortingInfo = null;
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.SortDirection is SortDirection.Ascending ? "ASC" : "DESC");
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);
        return new GetInvitationsRequest(invitationIdFilter, inviterIdFilter, inviteeIdFilter,
            locationFilter, dateOfMeetingFilter, createdOnFilter, activeToFilter, isAcceptedFilter, sortingInfo, paginationInfo,
            UserState.Value.User.Login);
    }
}