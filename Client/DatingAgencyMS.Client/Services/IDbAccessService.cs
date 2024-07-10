using DatingAgencyMS.Client.Models.DTOs.Auth;
using Refit;

namespace DatingAgencyMS.Client.Services;

public interface IDbAccessService
{
    [Post("/dbaccess/login")]
    Task<LoginResponse> Login(LoginRequest loginRequest);
}