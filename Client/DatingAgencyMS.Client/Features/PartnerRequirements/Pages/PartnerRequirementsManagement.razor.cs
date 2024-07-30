using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Pages;

public partial class PartnerRequirementsManagement : ComponentBase
{
    [Inject]
    private IState<UserState> UserState { get; init; }
    
}