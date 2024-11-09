using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Complaints.Models.Requests;
using DatingAgencyMS.Client.Features.Complaints.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Complaints.Pages;

public partial class UpdateComplaint : ComponentBase
{
    [Parameter] public int Id { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; } = null!;
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private IComplaintsService ComplaintsService { get; init; } = null!;
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    private LoggedInUser? _loggedInUser;
    private UpdateComplaintRequest? _request = new();
    
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
            var dto = await  ComplaintsService.GetByIdAsync(Id, _loggedInUser.Token);
            _request = new UpdateComplaintRequest()
            {
                Text = dto.Text,
                ComplaintId = Id
            };
        }
        await base.OnParametersSetAsync();
    }

    private async Task OnValidSubmit()
    {
        if (_request is null) return;

        try
        {
            await ComplaintsService.UpdateAsync(Id, _request, _loggedInUser!.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Запис скарги було успішно оновлено, переводимо вас назад."));
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/tables/complaints");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}