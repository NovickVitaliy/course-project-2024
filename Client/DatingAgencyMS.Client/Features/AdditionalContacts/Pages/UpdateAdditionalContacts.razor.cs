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

public partial class UpdateAdditionalContacts : ComponentBase
{
    [Parameter] public int Id { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IAdditionalContactsService AdditionalContactsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    private UpdateAdditionalContactsRequest? UpdateAdditionalContactsRequest { get; set; } = new();
    
    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_,_) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_loggedInUser is not null)
        {
            var additionalContacts = (await AdditionalContactsService.GetByIdAsync(Id, _loggedInUser.Token)).AdditionalContacts;
            UpdateAdditionalContactsRequest = new UpdateAdditionalContactsRequest()
            {
                Id = Id,
                Facebook = additionalContacts.Facebook,
                Instagram = additionalContacts.Instagram,
                Telegram = additionalContacts.Telegram,
                TikTok = additionalContacts.TikTok
            };
        }
        await base.OnParametersSetAsync();
    }


    private async Task OnValidSubmit()
    {
        if (UpdateAdditionalContactsRequest is null || UserState.Value.User is null) return;

        try
        {
            await AdditionalContactsService.UpdateAsync(Id, UpdateAdditionalContactsRequest, UserState.Value.User.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Дані було успішно оновлено. Переводимо вас назад до списку"));
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