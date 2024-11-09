using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.MeetingReviews.Models;
using DatingAgencyMS.Client.Features.MeetingReviews.Models.Requests;
using DatingAgencyMS.Client.Features.MeetingReviews.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.MeetingReviews.Components;

public partial class MeetingReviewsList : ComponentBase
{
    [Inject] private IMeetingReviewsService MeetingReviewsService { get; init; } = null!;
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    private LoggedInUser? _loggedInUser;
    private Grid<MeetingReviewDto>? _grid;
    private ConfirmDialog? _confirmDialog;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    private async Task<GridDataProviderResult<MeetingReviewDto>> MeetingReviewsDataProvider(GridDataProviderRequest<MeetingReviewDto> request)
    {
        var getRequest = BuildRequest(request);

        try
        {
            var response = await MeetingReviewsService.GetMeetingReviewsAsync(getRequest, _loggedInUser!.Token);
            return new GridDataProviderResult<MeetingReviewDto>
            {
                Data = response.MeetingReviews,
                TotalCount = (int?)response.Count
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<MeetingReviewDto>
            {
                Data = [],
                TotalCount = 0
            };
        }
    }
    private GetMeetingReviewsRequest BuildRequest(GridDataProviderRequest<MeetingReviewDto> request)
    {
        var idFilter = request.Filters.GetIntegerFilter("Id");
        var inviterScoreFilter = request.Filters.GetIntegerFilter("InviterScore");
        var inviterReviewFilter = request.Filters.GetStringFilter("InviterReview");
        var inviteeScoreFilter = request.Filters.GetIntegerFilter("InviteeScore");
        var inviteeReviewFilter = request.Filters.GetStringFilter("InviteeReview");
        var meetingIdFilter = request.Filters.GetIntegerFilter("MeetingId");
        SortingInfo? sortingInfo = null;
        var sorting = request.Sorting.FirstOrDefault();
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.GetSortDirection());
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);

        return new GetMeetingReviewsRequest(idFilter, inviterScoreFilter, inviterReviewFilter, inviteeScoreFilter,
            inviteeReviewFilter, meetingIdFilter, sortingInfo, paginationInfo);
    }
    private async Task DeleteMeetingReview(int id)
    {
        if (_confirmDialog is null) return;
        
        var confirmation = await _confirmDialog.ShowAsync(
            "Видалення відгуку про зустріч", 
            "Ви справді хочете видалити даний відгук про зустріч?",
            new()
            {
                YesButtonText = "Видалити",
                YesButtonColor = ButtonColor.Danger,
                NoButtonText = "Назад",
                NoButtonColor = ButtonColor.Secondary
            });

        if (confirmation)
        {
            try
            {
                await MeetingReviewsService.DeleteMeetingReviewAsync(id, _loggedInUser!.Token);
                ToastService.Notify(new ToastMessage(ToastType.Success, "Відгук про зустріч було успішно видалено"));
                await _grid!.RefreshDataAsync();
            }
            catch (ApiException e)
            {
                var apiError = e.ToApiError();
                ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            }
        }
    }
}