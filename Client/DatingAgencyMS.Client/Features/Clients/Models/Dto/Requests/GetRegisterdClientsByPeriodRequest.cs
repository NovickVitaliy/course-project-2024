using DatingAgencyMS.Client.Constants;

namespace DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;

public record GetRegisterdClientsByPeriodRequest(RegisteredByPeriod Period, int PageNumber, int PageSize, string RequestedBy);