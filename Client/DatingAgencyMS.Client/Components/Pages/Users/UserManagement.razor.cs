using BlazorBootstrap;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Models.DTOs.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DatingAgencyMS.Client.Components.Pages.Users;

public partial class UserManagement : ComponentBase
{
    [Inject]
    public IJSRuntime JsRuntime { get; set; }
    private readonly List<DbUser> _users = [
        new DbUser(1, "user1", "Admin"),
        new DbUser(2, "user2", "User"),
        new DbUser(3, "user3", "Admin"),
        new DbUser(4, "user4", "User"),
        new DbUser(5, "user5", "User"),
        new DbUser(6, "user6", "Admin"),
        new DbUser(7, "user7", "User"),
        new DbUser(8, "user8", "Admin"),
        new DbUser(9, "user9", "User"),
        new DbUser(10, "user10", "User"),
        new DbUser(11, "user11", "Admin"),
        new DbUser(12, "user12", "User"),
        new DbUser(13, "user13", "Admin"),
        new DbUser(14, "user14", "User"),
        new DbUser(15, "user15", "User"),
        new DbUser(16, "user16", "Admin"),
        new DbUser(17, "user17", "User"),
        new DbUser(18, "user18", "Admin"),
        new DbUser(19, "user19", "User"),
        new DbUser(20, "user20", "User")
    ];
}