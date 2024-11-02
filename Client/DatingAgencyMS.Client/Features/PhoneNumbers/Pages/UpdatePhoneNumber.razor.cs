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

public partial class UpdatePhoneNumber
{
    [Parameter] public int Id { get;init; }
    [Inject] private IPhoneNumbersService PhoneNumbersService { get; init; } = null!;
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private NavigationManager NavigationManager { get; init; } = null!;
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    private LoggedInUser? _loggedInUser;
    private UpdatePhoneNumberRequest _request = new();
    
    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User; 
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_loggedInUser is not null)
        {
            var dto = await PhoneNumbersService.GetByIdAsync(Id, _loggedInUser.Token);
            _request = new UpdatePhoneNumberRequest(){Id = Id, PhoneNumber = dto.PhoneNumber};
        }
        
        await base.OnParametersSetAsync();
    }
    private async Task OnValidSubmit()
    {
        if(_loggedInUser is null) return;

        try
        {
            await PhoneNumbersService.UpdateAsync(Id, _request, _loggedInUser.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Номер телефону було успішно оновлено. Перевожу назад до списку"));
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