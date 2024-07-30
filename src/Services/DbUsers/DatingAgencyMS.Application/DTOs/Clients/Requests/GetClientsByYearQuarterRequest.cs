using Common.Filtering.Pagination;

namespace DatingAgencyMS.Application.DTOs.Clients.Requests;

public record GetClientsByYearQuarterRequest(PaginationInfo PaginationInfo, int Year, int Quarter, string RequestedBy);