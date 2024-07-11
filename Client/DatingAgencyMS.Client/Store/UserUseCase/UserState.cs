using DatingAgencyMS.Client.Models.Core;
using Fluxor;

namespace DatingAgencyMS.Client.Store.UserUseCase;

[FeatureState]
public class UserState
{
    public User? User { get; }
    
    private UserState() {}

    public UserState(User? user)
    {
        User = user;
    }
}