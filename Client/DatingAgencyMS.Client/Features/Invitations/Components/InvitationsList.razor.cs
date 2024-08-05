using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Invitations.Models.Dtos;
using DatingAgencyMS.Client.Features.Invitations.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Invitations.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Invitations.Components;

public partial class InvitationsList : FluxorComponent
{
    [Inject] private IInvitationsService InvitationsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser = null;
    [Inject] private ToastService ToastService { get; init; }
    private Grid<InvitationDto>? _grid;
    private ConfirmDialog? _confirmDialog;
    private ConfirmDialogOptions _confirmDialogOptions = new()
    {
        YesButtonText = "Видалити",
        YesButtonColor = ButtonColor.Danger,
        NoButtonText = "Назад",
        NoButtonColor = ButtonColor.Secondary
    };

    protected override void OnParametersSet()
    {
        UserState.StateChanged += (sender, args) =>
        {
            _loggedInUser = UserState.Value.User;
        };
        base.OnParametersSet();
    }

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

    private async Task DeleteInvitation(int invitationId)
    {
        if (_confirmDialog is null) return;

        var confirmation = await _confirmDialog.ShowAsync(
            "Підтвердження видалення", 
            $"Ви справді хочете видалити запрошення з Id = {invitationId}",
            _confirmDialogOptions);

        if (!confirmation) return;

        try
        {
            await InvitationsService.DeleteInvitation(invitationId, UserState.Value.User.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, $"Запрошення з Id - {invitationId} було успішно видалено"));
            _grid?.RefreshDataAsync();
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}