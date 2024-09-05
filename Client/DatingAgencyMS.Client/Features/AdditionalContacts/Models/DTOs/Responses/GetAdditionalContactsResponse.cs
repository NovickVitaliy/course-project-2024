namespace DatingAgencyMS.Client.Features.AdditionalContacts.Models.DTOs.Responses;

public record GetAdditionalContactsResponse(IReadOnlyList<AdditionalContactDto> AdditionalContacts, long TotalCount);