using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Client.Features.ClientRatings.Models.Requests;

public record GetClientRatingsRequest(
    IntegerFilter IdFilter,
    IntegerFilter ClientIdFilter,
    IntegerFilter RatingFilter,
    StringFilter CommentFilter,
    DateTimeFilter RatingDateFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo);