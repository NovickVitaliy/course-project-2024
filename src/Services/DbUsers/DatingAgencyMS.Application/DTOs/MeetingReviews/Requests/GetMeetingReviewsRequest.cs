using Common.Filtering.Filters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.MeetingReviews.Requests;

public record GetMeetingReviewsRequest(
    IntegerFilter IdFilter,
    IntegerFilter InviterScoreFilter,
    StringFilter InviterReviewFilter,
    IntegerFilter InviteeScoreFilter,
    StringFilter InviteeReviewFilter,
    IntegerFilter MeetingIdFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo
);