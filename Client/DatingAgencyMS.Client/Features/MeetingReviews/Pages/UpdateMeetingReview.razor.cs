using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.MeetingReviews.Models.Requests;
using DatingAgencyMS.Client.Features.MeetingReviews.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.MeetingReviews.Pages;

public partial class UpdateMeetingReview : ComponentBase
{
    [Parameter] public int Id { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; } = null!;
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    [Inject] private IMeetingReviewsService MeetingReviewsService { get; init; } = null!;
    private UpdateMeetingReviewRequest? _request = new();

    protected override async Task OnParametersSetAsync()
    {
        await SetUpdateRequest();
        await base.OnParametersSetAsync();
    }
    private async Task SetUpdateRequest()
    {
        var dto = await MeetingReviewsService.GetMeetingReviewByIdAsync(Id, UserState.Value.User!.Token);
        _request = new UpdateMeetingReviewRequest
        {
            InviteeReview = dto.InviteeReview,
            InviteeScore = dto.InviteeScore,
            Id = Id,
            InviterReview = dto.InviterReview,
            InviterScore = dto.InviterScore,
            MeetingId = dto.MeetingId
        };
    }
    private async Task OnValidSubmit()
    {
        if (_request is null) return;

        try
        {
            await MeetingReviewsService.UpdateMeetingReviewAsync(Id, _request, UserState.Value.User!.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Запис було успішно оновлено. Переводжу назад до списку"));
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/tables/meeting-reviews");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}