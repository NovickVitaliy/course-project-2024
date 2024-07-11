using Blazored.LocalStorage;
using DatingAgencyMS.Client.Constants;
using Fluxor;

namespace DatingAgencyMS.Client.Store.UserUseCase;

public static class UserReducers
{
    [ReducerMethod]
    public static UserState SetUser(UserState state, SetUserAction action) => new(action.User);
}