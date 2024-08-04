using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.Invitations.Pages;

public partial class InvitationsManagement : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; }
}