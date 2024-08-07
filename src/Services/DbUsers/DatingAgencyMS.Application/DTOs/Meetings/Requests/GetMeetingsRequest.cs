using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.Meetings.Requests;

public record GetMeetingsRequest(
    IntegerFilter MeetingIdFilter,
    DateTimeFilter DateFilter,
    IntegerFilter InviterIdFilter,
    IntegerFilter InviteeIdFilter,
    StringFilter LocationFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo);