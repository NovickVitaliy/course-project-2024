using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Attributes.DataTypes;

namespace DatingAgencyMS.Client.Features.PhoneNumbers.Models.Requests;

public class CreatePhoneNumberRequest
{
    [UkrainianPhoneNumber(ErrorMessage = "Номер телефону повинен бути вказаний в українському форматі")]
    [Required(ErrorMessage = "Номер телефону є обов'язковим")]
    public string PhoneNumber { get; set; } = null!;
    
    [Range(0, int.MaxValue, ErrorMessage = "Id об'єкту додаткових контактів повинно бути більше 0")]
    [Required(ErrorMessage = "Id об'єкту додаткових контактів є обов'язковим")]
    public int AdditionalContactsId { get; set; }
}