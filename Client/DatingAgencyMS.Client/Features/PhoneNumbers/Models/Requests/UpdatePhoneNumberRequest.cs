using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Attributes.DataTypes;

namespace DatingAgencyMS.Client.Features.PhoneNumbers.Models.Requests;

public class UpdatePhoneNumberRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Номер телефону не може бути пустим")]
    [UkrainianPhoneNumber(ErrorMessage = "Номер телефону повинен бути вказаний в українському форматі")]
    public string PhoneNumber { get; set; } = null!;
}