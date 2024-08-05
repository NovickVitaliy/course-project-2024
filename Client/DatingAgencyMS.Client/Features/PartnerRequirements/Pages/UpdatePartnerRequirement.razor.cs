using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Features.PartnerRequirements.Helpers;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.PartnerRequirements.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Pages;

public partial class UpdatePartnerRequirement
{
    [Parameter] public int Id { get; init; }
    [Inject] private IPartnerRequirementsService PartnerRequirementsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    private UpdatePartnerRequirementRequest? _request = new();
    
    protected override async Task OnInitializedAsync()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        if (_loggedInUser != null)
        {
            var response = await PartnerRequirementsService.GetPartnerRequirementById(Id, _loggedInUser.Token);
            _request = response.PartnerRequirements.ToUpdateRequest();
            _request.RequestedBy = _loggedInUser.Login;
        }
        await base.OnInitializedAsync();
    }

    private async Task OnValidSubmit(EditContext obj)
    {
        if (_loggedInUser is null || _request is null) return;
        try
        {
            await PartnerRequirementsService.UpdatePartnerRequirements(Id, _request, _loggedInUser.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Дані про вимоги до партнера були успішно оновлені." +
                                                                   "Напрвляємо вас назад до списку"));
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