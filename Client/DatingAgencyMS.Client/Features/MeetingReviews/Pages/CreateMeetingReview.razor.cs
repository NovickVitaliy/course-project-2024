using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.MeetingReviews.Models.Requests;
using DatingAgencyMS.Client.Features.MeetingReviews.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.MeetingReviews.Pages;

public partial class CreateMeetingReview : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    [Inject] private IMeetingReviewsService MeetingReviewsService { get; init; } = null!;
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private NavigationManager NavigationManager { get; init; } = null!;
    private readonly CreateMeetingReviewRequest? _request = new();
    
    private async Task OnValidSubmit()
    {
        if (_request is null) return;

        try
        {
            await MeetingReviewsService.CreateMeetingReviewAsync(_request, UserState.Value.User!.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, 
                "Відгук про зустріч було успішно створено. Перевожу до списку."));
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