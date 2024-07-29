using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;

public class UpdateClientRequest
{
    [Required(ErrorMessage = "Ім'я не може бути відстунім")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Прізвище не може бути відстунім")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Гендер не може бути відстунім")]
    public string Gender { get; set; }
    
    [Required(ErrorMessage = "Стать не може бути відсутня")]
    public string Sex { get; set; }
    
    [Required(ErrorMessage = "Сексуальна орієнтація не може бути відсутньою")]
    public string SexualOrientation { get; set; }
    
    [Required(ErrorMessage = "Реєстраційний номер не може бути відстунім")]
    public string RegistrationNumber { get; set; }
    
    [Required(ErrorMessage = "Вік не може бути відстунім")]
    [Range(18, 90, ErrorMessage = "Вік повинен бути в межах від 18 до 90 років")]
    public int Age { get; set; }
    
    [Required(ErrorMessage = "Ріст не може бути відстунім")]
    [Range(0, 300, ErrorMessage = "Ріст повинен бути в межах від 0 до 300 см")]
    public int Height { get; set; }
    
    [Required(ErrorMessage = "Вага не може бути відстутньою")]
    [Range(0, 300, ErrorMessage = "Вага повинна бути в межах від 0 до 300 кг")]
    public int Weight { get; set; }
    
    [Required(ErrorMessage = "Знак Зодіаку не може бути відсутнім")]
    public ZodiacSign ZodiacSign { get; set; }

    [Required(ErrorMessage = "Опис не може бути відсутнім")]
    [MaxLength(255)]
    public string Description { get; set; } = "";

    public bool HasDeclinedService { get; set; } = false;
    
    public string RequestedBy { get; set; }
};