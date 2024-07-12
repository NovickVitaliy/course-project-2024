using DatingAgencyMS.Client.Models.Core;
using Fluxor;

namespace DatingAgencyMS.Client.Store.UserUseCase;

[FeatureState]
public class UserState
{
    public LoggedInUser? User { get; }
    
    private UserState() {}

    public UserState(LoggedInUser? user)
    {
        User = user;
    }
}