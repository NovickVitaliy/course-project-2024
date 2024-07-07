namespace DatingAgencyMS.Application.Contracts;

public interface ITokenService
{
    Task<string> GenerateJwtToken(string login);
}