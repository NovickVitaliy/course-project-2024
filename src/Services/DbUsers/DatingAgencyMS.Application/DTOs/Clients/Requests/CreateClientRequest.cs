using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Application.DTOs.Clients.Requests;

public record CreateClientRequest(
    string FirstName,
    string LastName,
    string Gender,
    string SexualOrientation,
    string RegistrationNumber,
    int Age,
    int Height,
    int Weight,
    ZodiacSign ZodiacSign,
    string Description,
    string RequestedBy);