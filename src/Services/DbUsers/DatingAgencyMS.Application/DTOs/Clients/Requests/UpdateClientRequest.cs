using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Application.DTOs.Clients.Requests;

public record UpdateClientRequest(
    string FirstName,
    string LastName,
    string Gender,
    string Sex,
    string SexualOrientation,
    string RegistrationNumber,
    int Age,
    int Height,
    int Weight,
    ZodiacSign ZodiacSign,
    string Description,
    bool HasDeclinedService,
    string RequestedBy);