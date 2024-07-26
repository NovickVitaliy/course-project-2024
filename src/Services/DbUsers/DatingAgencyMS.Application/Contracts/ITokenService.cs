using DatingAgencyMS.Domain.Models;
using DatingAgencyMS.Domain.Models.DbManagement;

namespace DatingAgencyMS.Application.Contracts;

public interface ITokenService
{
    Task<string> GenerateJwtToken(string login, DbRoles role);
}