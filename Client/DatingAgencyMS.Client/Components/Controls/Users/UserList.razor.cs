using BlazorBootstrap;
using DatingAgencyMS.Client.Models.DTOs.User;
using DatingAgencyMS.Client.Models.DTOs.User.Requests;
using DatingAgencyMS.Client.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DatingAgencyMS.Client.Components.Controls.Users;

public partial class UserList : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<DbUser>? Users { get; set; }
    
    [Inject]
    public IJSRuntime JsRuntime { get; set; }
    
    [Inject]
    private IUsersService UsersService { get; set; }
    
    [Inject]
    private IState<UserState> UserState { get; set; }
    private async Task<GridDataProviderResult<DbUser>> UserDataProvider(GridDataProviderRequest<DbUser> request)
    {
        var id = request.Filters.FirstOrDefault(x => x.PropertyName == "Id")?.Value;
        var login = request.Filters.FirstOrDefault(x => x.PropertyName == "Login")?.Value;
        var role = request.Filters.FirstOrDefault(x => x.PropertyName == "Role")?.Value;
        var page = request.PageNumber;
        var size = request.PageSize;
        var sort = request.Sorting.FirstOrDefault();
        string? sortBy = null;
        string? sortDirection = null;
        if (sort is not null)
        {
            sortBy = sort.SortString;
            sortDirection = sort.SortDirection == SortDirection.Ascending ? "ASC" : "DESC";
        }
        
        var req = new GetUsersRequest(
            id == null ? null : int.Parse(id), 
            login, 
            role,
            page,
            size,
            sortBy,
            sortDirection,
            UserState.Value.User.Login);
        var users = await UsersService.GetUsers(req, UserState.Value.User.Token);
        return new GridDataProviderResult<DbUser>() { Data = users.Users, TotalCount = (int?)users.TotalCount};
    }
}