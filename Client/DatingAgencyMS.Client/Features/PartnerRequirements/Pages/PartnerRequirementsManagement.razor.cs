using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Pages;

public partial class PartnerRequirementsManagement
{
    [Inject]
    private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) =>
        {
            _loggedInUser = UserState.Value.User;
        };
        base.OnInitialized();
    }
}