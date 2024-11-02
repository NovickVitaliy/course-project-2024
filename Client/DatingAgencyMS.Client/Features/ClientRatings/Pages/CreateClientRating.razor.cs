using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.ClientRatings.Models.Requests;
using DatingAgencyMS.Client.Features.ClientRatings.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.ClientRatings.Pages;

public partial class CreateClientRating : ComponentBase
{
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private IClientRatingsService ClientRatingsService { get; init; } = null!;
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    [Inject] private NavigationManager NavigationManager { get; init; } = null!;
    private LoggedInUser? _loggedInUser = null;
    private readonly CreateClientRatingRequest? _request = new();
    
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
            await ClientRatingsService.CreateAsync(_request, UserState.Value.User!.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Відгук клієнта було успішно створено. Переводимо вас назад."));
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/table/client-ratings");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}