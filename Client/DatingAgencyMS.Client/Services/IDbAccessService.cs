using DatingAgencyMS.Client.Models.DTOs.Auth;
using Refit;

namespace DatingAgencyMS.Client.Services;

public interface IDbAccessService
{
    [Post("/dbaccess/login")]
    Task<LoginResponse> Login(LoginRequest loginRequest);

    [Post("/dbaccess/close/{login}")]
    Task Logout(string login, [Authorize]string token);

    [Post("/dbaccess/forgot-password")]
    Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request);
}