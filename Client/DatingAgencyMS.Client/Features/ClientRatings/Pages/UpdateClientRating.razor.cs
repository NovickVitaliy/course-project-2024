using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.ClientRatings.Models.Requests;
using DatingAgencyMS.Client.Features.ClientRatings.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.ClientRatings.Pages;

public partial class UpdateClientRating : ComponentBase
{
    [Parameter] public int Id { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; } = null!;
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private IClientRatingsService ClientRatingsService { get; init; } = null!;
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    private LoggedInUser? _loggedInUser = null;
    private UpdateClientRatingRequest? _request = new(); 
        
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
            var dto = await ClientRatingsService.GetByIdAsync(Id, UserState.Value.User!.Token);
            _request = new UpdateClientRatingRequest()
            {
                Comment = dto.Comment,
                Id = Id,
                Rating = dto.Rating
            };
        }
        
        await base.OnParametersSetAsync();
    }

    private async Task OnValidSubmit()
    {
        if (_request is null) return;

        try
        {
            await ClientRatingsService.UpdateAsync(Id, _request, _loggedInUser!.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Відгук клієнта було успішно оновлено. Переводжу назад"));
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/tables/client-ratings");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}