using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.SqlQueries.Models;
using DatingAgencyMS.Client.Features.SqlQueries.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.SqlQueries.Pages;

public partial class SqlQueries : ComponentBase
{
  [Inject]
  private IState<UserState> UserState { get; init; } = null!;

  [Inject]
  private ISqlQueryService SqlQueryService { get; init; } = null!;

  [Inject] 
  private ToastService ToastService { get; init; } = null!;
  
  public string EditorContent { get; set; }

    private string[]? _sqlServiceResult = null!;

    private async Task Process()
    {
        try
        {
            var response = await SqlQueryService.RunSqlQueryAsync(new SqlQueryRequest(EditorContent), UserState.Value.User!.Token);
            if (response.Response.Length == 1)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, response.Response[0]));
                return;
            }

            _sqlServiceResult = response.Response;
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}