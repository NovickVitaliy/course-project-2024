using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.ClientRatings.Requests;

public record GetClientRatingsRequest(
    IntegerFilter IdFilter,
    IntegerFilter ClientIdFilter,
    IntegerFilter RatingFilter,
    StringFilter CommentFilter,
    DateTimeFilter RatingDateFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo);