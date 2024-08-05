using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.Invitations.Pages;

public partial class InvitationsManagement
{
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;

    protected override void OnInitialized()
    {
        UserState.StateChanged += (sender, args) =>
        {
            _loggedInUser = UserState.Value.User;
        };
        base.OnInitialized();
    }
}