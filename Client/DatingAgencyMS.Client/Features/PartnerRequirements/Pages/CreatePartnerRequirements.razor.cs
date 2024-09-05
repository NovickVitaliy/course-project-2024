using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.PartnerRequirements.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Pages;

public partial class CreatePartnerRequirements
{
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    [Inject] private IPartnerRequirementsService PartnerRequirementsService { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    private CreatePartnerRequirementsRequest _request = new();

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (_loggedInUser is not null)
        {
            _request = new CreatePartnerRequirementsRequest()
            {
                RequestedBy = _loggedInUser.Login
            };
        }
        base.OnParametersSet();
    }

    private async Task OnValidSubmit()
    {
        if (_loggedInUser is null) return;
        try
        {
            await PartnerRequirementsService.CreatePartnerRequirements(_request, _loggedInUser.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Об'єкт вимог до партнера було успішно створено. Переводимо назад до таблиці"));
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/tables/partner-requirements");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}