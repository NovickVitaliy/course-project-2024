using DatingAgencyMS.Domain.Models;

namespace DatingAgencyMS.Application.Contracts;

public interface ITokenService
{
    Task<string> GenerateJwtToken(string login, DbRoles role);
}