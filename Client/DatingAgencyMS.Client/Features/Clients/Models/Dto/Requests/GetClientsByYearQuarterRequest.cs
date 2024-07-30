using Common.Filtering.Pagination;

namespace DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;

public record GetClientsByYearQuarterRequest(PaginationInfo PaginationInfo, int Year, int Quarter, string RequestedBy);