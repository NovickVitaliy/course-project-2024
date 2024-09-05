using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Models.DTOs.Auth;
using DatingAgencyMS.Client.Services;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Components.Pages.Auth;

public partial class ForgotPassword : ComponentBase
{
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IDbAccessService DbAccessService { get; init; }
    [SupplyParameterFromForm] private ForgotPasswordRequest ForgotPasswordRequest { get; set; } = new();
    private string? _password = null;
    private async Task OnValidSubmit()
    {
        try
        {
            var response = await DbAccessService.ForgotPassword(ForgotPasswordRequest);
            _password = response.Password;
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}