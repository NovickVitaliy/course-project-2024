using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.CoupleArchive.Pages;

public partial class CoupleArchiveManagement
{
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_,_) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }
}