using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Complaints.Models.Requests;
using DatingAgencyMS.Client.Features.Complaints.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Complaints.Pages;

public partial class CreateComplaint : ComponentBase
{
    [Inject] private IComplaintsService ComplaintsService { get; init; } = null!;
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private IComplaintsService ClientRatingsService { get; init; } = null!;
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    [Inject] private NavigationManager NavigationManager { get; init; } = null!;
    private LoggedInUser? _loggedInUser = null;
    private readonly CreateComplaintRequest? _request = new();
    
    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    private async Task OnValidSubmit()
    {
        if (_request is null) return;

        try
        {
            await ComplaintsService.CreateAsync(_request, _loggedInUser!.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Скарга була успішно створена, переводимо вас назад до списку."));
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/tables/complaints");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}