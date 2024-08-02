using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto;
using DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.PartnerRequirements.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Components;

public partial class PartnerRequirementsList : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; }
    [Inject] private IPartnerRequirementsService PartnerRequirementsService { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    private ConfirmDialog _confirmDialog = default!;
    private Grid<PartnerRequirementsDto> _grid = default!;
    
    private async Task<GridDataProviderResult<PartnerRequirementsDto>> PartnerRequirementsDataProvider(
        GridDataProviderRequest<PartnerRequirementsDto> request)
    {
        var partnersRequirementsRequest = BuildRequest(request);
        var response =
            await PartnerRequirementsService.GetPartnerRequirements(partnersRequirementsRequest,
                UserState.Value.User.Token);
        
        return new GridDataProviderResult<PartnerRequirementsDto>()
        {
            Data = response.PartnerRequirements,
            TotalCount = (int?)response.Count
        };
    }

    private GetPartnersRequirementRequest BuildRequest(GridDataProviderRequest<PartnerRequirementsDto> request)
    {
        var idFilter = request.Filters.GetIntegerFilter("Id");
        var genderFilter = request.Filters.GetStringFilter("Gender");
        var sexFilter = request.Filters.GetStringFilter("Sex");
        var minAgeFilter = request.Filters.GetIntegerFilter("MinAge");
        var maxAgeFilter = request.Filters.GetIntegerFilter("MaxAge");
        var minHeightFilter = request.Filters.GetIntegerFilter("MinHeight");
        var maxHeightFilter = request.Filters.GetIntegerFilter("MaxHeight");
        var minWeightFilter = request.Filters.GetIntegerFilter("MinWeight");
        var maxWeightFilter = request.Filters.GetIntegerFilter("MaxWeight");
        var zodiacSignFilter = request.Filters.GetStringFilter("ZodiacSign");
        var locationFilter = request.Filters.GetStringFilter("Location");
        var clientIdFilter = request.Filters.GetIntegerFilter("ClientId");
        var sorting = request.Sorting.FirstOrDefault();
        SortingInfo? sortingInfo = null;
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.SortDirection == SortDirection.Ascending
                ? "ASC"
                : "DESC");
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);
        return new GetPartnersRequirementRequest(idFilter, genderFilter, sexFilter, minAgeFilter, maxAgeFilter,
            minHeightFilter,
            maxHeightFilter, minWeightFilter, maxWeightFilter, zodiacSignFilter, locationFilter,  clientIdFilter, sortingInfo,
            paginationInfo, UserState.Value.User.Login);
    }

    private async Task DeletePartnerRequirements(int id)
    {
        var confirmation = await _confirmDialog.ShowAsync(
            title:$"Ви сравді хочете видалити об'єкт вимог до партнера з Id - {id}?",
            message1:"Це видалить запис з БД. Ця дія незворотня.",
            confirmDialogOptions: new ConfirmDialogOptions()
            {
                YesButtonText = "Видалити",
                YesButtonColor = ButtonColor.Danger,
                NoButtonText = "Назад",
                NoButtonColor = ButtonColor.Secondary
            });

        if (confirmation)
        {
            try
            {
                await PartnerRequirementsService.DeletePartnerRequirements(id, UserState.Value.User.Token);
                await _grid.RefreshDataAsync();
            }
            catch (ApiException e)
            {
                var apiError = e.ToApiError();
                ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            }
        }
    }

    private async Task GetMatchesCount(PartnerRequirementsDto partnerRequirementsDto)
    {
        try
        {
            var count = await PartnerRequirementsService.GetMatchesCount(partnerRequirementsDto.Id, UserState.Value.User.Token);
            await _confirmDialog.ShowAsync("Кількість партнерів за вимогами", $"Кількість партнерів що підходять за вимоги для " +
                                                                       $"клієнта з Id {partnerRequirementsDto.ClientId} - {count} людей",
                new ConfirmDialogOptions
                {
                    IsVerticallyCentered = true,
                    YesButtonText = "OK",
                    NoButtonColor = ButtonColor.None,
                    NoButtonText = string.Empty
                });
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}