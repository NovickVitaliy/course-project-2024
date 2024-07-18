using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Models.DTOs.User;
using DatingAgencyMS.Client.Models.DTOs.User.Requests;
using DatingAgencyMS.Client.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Components.Pages.Users;

public partial class AssignNewRole : ComponentBase
{
    private string _userLogin = "";
    private string _errorMessage = "";
    [Inject] private IUsersService UsersService { get; init; }

    [Inject] private IState<UserState> UserState { get; init; }
    
    [SupplyParameterFromForm]
    private AssignNewRoleRequest? _assignNewRoleRequest { get; set; } = null;
    
    private async Task LoadUser()
    {
        if (!string.IsNullOrEmpty(_userLogin))
        {
            _errorMessage = string.Empty;
            try
            {
                var response = await UsersService.GetUser(_userLogin, UserState.Value.User.Token);
                var user = response.User;
                _assignNewRoleRequest = new AssignNewRoleRequest
                {
                    Login = user.Login,
                    OldRole = Enum.Parse<DbRoles>(user.Role, true),
                };
            }
            catch (ApiException e)
            {
                //notify
                _assignNewRoleRequest = null;
            }
        }
        else
        {
            _errorMessage = "Логін не може бути пустим";
        }
    }

    private bool IsOwner()
    {
        return UserState.Value.User!.Role == DbRoles.Owner;
    }
    
    private async Task OnValidSubmit()
    {
        _assignNewRoleRequest.RequestedBy = UserState.Value.User.Login;
        //TODO: send request
    }
}