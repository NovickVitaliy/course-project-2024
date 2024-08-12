using Common.Filtering.Filters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.Visits.Requests;

public record GetVisitsRequest(
    IntegerFilter IdFilter,
    IntegerFilter ClientIdFilter,
    IntegerFilter MeetingIdFilter,
    BooleanFilter VisitedFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo
);