using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.AdditionalContacts.Models.DTOs.Requests;
using DatingAgencyMS.Client.Features.AdditionalContacts.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.AdditionalContacts.Pages;

public partial class CreateAdditionalContacts : ComponentBase
{
    [SupplyParameterFromForm]
    private CreateAdditionalContactsRequest? CreateAdditionalContactsRequest { get; set; } = new();

    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IAdditionalContactsService AdditionalContactsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    private LoggedInUser? _loggedInUser = null;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    private async Task OnValidSubmit()
    {
        if (CreateAdditionalContactsRequest is null || UserState.Value.User is null) return;
        if (!(CreateAdditionalContactsRequest.Facebook is not null
              || CreateAdditionalContactsRequest.Instagram is not null
              || CreateAdditionalContactsRequest.Telegram is not null
              || CreateAdditionalContactsRequest.TikTok is not null))
        {
            ToastService.Notify(new ToastMessage(ToastType.Danger, "Як мінімум одне посилання на соц. мережу, має бути вказане"));
            return;
        }
        
        try
        {
            await AdditionalContactsService.CreateAsync(CreateAdditionalContactsRequest, UserState.Value.User.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Об'єкт додаткових контаків для користувача з ID " +
                                                                    $"{CreateAdditionalContactsRequest.ClientId} був успішно створений." +
                                                                    $" Переводимо вас назад до списку"));
            await Task.Delay(TimeSpan.FromSeconds(2));
            NavigationManager.NavigateTo("/tables/additional-contacts");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}