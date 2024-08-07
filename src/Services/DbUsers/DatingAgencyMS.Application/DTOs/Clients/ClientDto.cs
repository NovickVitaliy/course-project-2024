using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Application.DTOs.Clients;

public record ClientDto(
    int ClientId,
    string FirstName,
    string LastName,
    string Gender,
    string Sex,
    string SexualOrientation,
    string Location,
    string RegistrationNumber,
    DateOnly RegisteredOn,
    int Age,
    int Height,
    int Weight,
    ZodiacSign ZodiacSign,
    string Description,
    bool HasDeclinedService);