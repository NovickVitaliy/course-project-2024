using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Extensions;

public static class ClientExtensions
{
    public static UpdateClientRequest ToUpdateClientRequest(this ClientDto clientDto)
        => new UpdateClientRequest()
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            ZodiacSign = clientDto.ZodiacSign,
            Age = clientDto.Age,
            Description = clientDto.Description,
            Gender = clientDto.Gender,
            Height = clientDto.Height,
            Weight = clientDto.Weight,
            RegistrationNumber = clientDto.RegistrationNumber,
            SexualOrientation = clientDto.SexualOrientation
        };
}