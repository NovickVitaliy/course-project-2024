using BlazorBootstrap;
using DatingAgencyMS.Client.Models.DTOs.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DatingAgencyMS.Client.Components.Controls.Users;

public partial class UserList : ComponentBase
{
    [Parameter, EditorRequired]
    public IEnumerable<DbUser>? Users { get; set; }
    
    [Inject]
    public IJSRuntime JsRuntime { get; set; }
    
    private async Task<GridDataProviderResult<DbUser>> UserDataProvider(GridDataProviderRequest<DbUser> request)
    {
        //TODO: fetch users with param from api
        Users ??= [];
        await JsRuntime.InvokeVoidAsync("console.log", "hello");
        return await Task.FromResult(request.ApplyTo(Users));
    }
}