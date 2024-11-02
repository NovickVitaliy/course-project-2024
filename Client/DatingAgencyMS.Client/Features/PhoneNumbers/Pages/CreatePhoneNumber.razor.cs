using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.PhoneNumbers.Models.Requests;
using DatingAgencyMS.Client.Features.PhoneNumbers.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.PhoneNumbers.Pages;

public partial class CreatePhoneNumber
{
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IPhoneNumbersService PhoneNumbersService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    private readonly CreatePhoneNumberRequest? _request = new();
    
    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }
    
    private async Task OnValidSubmit()
    {
        if (_request is not null)
        {
            try
            {
                await PhoneNumbersService.CreateAsync(_request, _loggedInUser!.Token);
                ToastService.Notify(new ToastMessage(ToastType.Success, "Номер телефону було успішно створено. Перевожу назад до списку номерів телефонів"));
                await Task.Delay(2000);
                NavigationManager.NavigateTo("/tables/phone-numbers");
            }
            catch (ApiException e)
            {
                var apiError = e.ToApiError();
                ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            }
        }
    }
}