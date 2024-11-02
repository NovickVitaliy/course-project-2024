using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DatingAgencyMS.Client.Attributes.DataTypes;

public class UkrainianPhoneNumberAttribute : RegularExpressionAttribute
{
    [StringSyntax(StringSyntaxAttribute.Regex)] 
    private const string UkrainianPhoneNumberPattern = @"^\+?3?8?(0\d{9})$";
    
    public UkrainianPhoneNumberAttribute() : base(UkrainianPhoneNumberPattern)
    { }
}