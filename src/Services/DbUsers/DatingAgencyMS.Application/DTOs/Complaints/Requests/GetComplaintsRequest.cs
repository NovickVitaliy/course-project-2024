using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.Complaints.Requests;

public record GetComplaintsRequest(
    IntegerFilter ComplaintIdFilter,
    IntegerFilter ComplainantIdFilter,
    IntegerFilter ComplaineeIdFilter,
    DateTimeFilter DateFilter,
    StringFilter TextFilter,
    StringFilter ComplaintStatusFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo);