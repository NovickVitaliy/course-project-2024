using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class ClientsManagement : ComponentBase
{
    [Inject]
    private IState<UserState> UserState { get; init; }
}