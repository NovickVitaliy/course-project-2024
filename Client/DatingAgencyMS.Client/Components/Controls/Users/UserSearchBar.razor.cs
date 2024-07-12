using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DatingAgencyMS.Client.Components.Controls.Users;

public partial class UserSearchBar : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; }
    
    [Parameter, EditorRequired]
    public EventCallback<string> OnSearchUser { get; set; }

    private string _userQuery = string.Empty;

    private async Task SearchUser()
    {
        await OnSearchUser.InvokeAsync(_userQuery);
    }
}