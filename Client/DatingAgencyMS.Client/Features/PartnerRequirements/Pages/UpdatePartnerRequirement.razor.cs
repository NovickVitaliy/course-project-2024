using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Features.PartnerRequirements.Helpers;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.PartnerRequirements.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Pages;

public partial class UpdatePartnerRequirement : ComponentBase
{
    [Parameter]
    public int Id { get; init; }
    
    [Inject] private IPartnerRequirementsService PartnerRequirementsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    private UpdatePartnerRequirementRequest? _request;

    protected override async Task OnInitializedAsync()
    {
        var response = await PartnerRequirementsService.GetPartnerRequirementById(Id, UserState.Value.User.Token);
        _request = response.PartnerRequirements.ToUpdateRequest();
        _request.RequestedBy = UserState.Value.User.Login;
        
        await base.OnInitializedAsync();
    }

    private async Task OnValidSubmit(EditContext obj)
    {
        try
        {
            await PartnerRequirementsService.UpdatePartnerRequirements(Id, _request, UserState.Value.User.Token);
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