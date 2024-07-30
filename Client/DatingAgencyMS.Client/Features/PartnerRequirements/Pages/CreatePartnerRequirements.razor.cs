using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.PartnerRequirements.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Pages;

public partial class CreatePartnerRequirements : ComponentBase
{
    [Inject]
    private IState<UserState> UserState { get; init; }
    
    [Inject]
    private IPartnerRequirementsService PartnerRequirementsService { get; init; }
    
    [Inject]
    private ToastService ToastService { get; init; }
    
    [Inject]
    private NavigationManager NavigationManager { get; init; }
    
    private CreatePartnerRequirementsRequest _request;

    protected override void OnParametersSet()
    {
        _request = new CreatePartnerRequirementsRequest()
        {
            RequestedBy = UserState.Value.User.Login
        };
        base.OnParametersSet();
    }

    private async Task OnValidSubmit()
    {
        try
        {
            await PartnerRequirementsService.CreatePartnerRequirements(_request, UserState.Value.User.Token);
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