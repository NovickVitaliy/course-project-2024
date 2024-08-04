using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.Invitations.Requests;

public record GetInvitationsRequest(
    IntegerFilter InvitationIdFilter,
    IntegerFilter InviterIdFilter,
    IntegerFilter InviteeIdFilter,
    StringFilter LocationFilter,
    DateTimeFilter DateOfMeetingFilter,
    DateOnlyFilter CreatedOnFilter,
    DateOnlyFilter ActiveToFilter,
    BooleanFilter IsAcceptedFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo,
    string RequestedBy);