using Common.Filtering.Filters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Client.Features.Visits.Models.Dtos.Requests;

public record GetVisitsRequest(
    IntegerFilter IdFilter,
    IntegerFilter ClientIdFilter,
    IntegerFilter MeetingIdFilter,
    BooleanFilter VisitedFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo);