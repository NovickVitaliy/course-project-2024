using DatingAgencyMS.Client.Constants;

namespace DatingAgencyMS.Application.DTOs.Clients.Requests;

public record GetClientsByTimePeriodRequest(RegisteredByPeriod Period, int PageNumber, int PageSize, string RequestedBy);