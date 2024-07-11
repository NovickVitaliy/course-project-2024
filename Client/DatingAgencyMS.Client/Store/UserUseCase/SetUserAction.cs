using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Store.UserUseCase;

public class SetUserAction
{
    public User? User { get; }

    public SetUserAction(User? user)
    {
        User = user;
    }
}