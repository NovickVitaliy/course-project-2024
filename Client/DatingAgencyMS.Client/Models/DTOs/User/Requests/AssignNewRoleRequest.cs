using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Attributes.ValidationAttributes;
using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Models.DTOs.User.Requests;

public class AssignNewRoleRequest
{
    [Required]
    public string Login { get; set; }
    
    [Required]
    public DbRoles OldRole { get; set; }
    
    [Required]
    [NotEquals(nameof(OldRole))]
    public DbRoles NewRole { get; set; }
    
    [Required]
    public string RequestedBy { get; set; }
}