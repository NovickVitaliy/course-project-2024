using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Store.UserUseCase;

public class SetUserAction
{
    public LoggedInUser? User { get; }

    public SetUserAction(LoggedInUser? user)
    {
        User = user;
    }
}