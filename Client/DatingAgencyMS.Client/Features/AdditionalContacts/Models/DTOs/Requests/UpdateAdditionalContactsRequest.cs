using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Attributes.DataTypes;

namespace DatingAgencyMS.Client.Features.AdditionalContacts.Models.DTOs.Requests;

public class UpdateAdditionalContactsRequest
{
    [Required]
    public int Id { get; set; }
    
    [Telegram]
    public string? Telegram { get; set; }
    
    [Facebook]
    public string? Facebook { get; set; }
    
    [Instagram]
    public string? Instagram { get; set; }
    
    [TikTok]
    public string? TikTok { get; set; }
}