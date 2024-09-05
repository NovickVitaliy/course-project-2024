using Common.Filtering.Filters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.AdditionalContacts.Requests;

public record GetAdditionalContactsRequest(
    IntegerFilter IdFilter,
    IntegerFilter ClientIdFilter,
    StringFilter TelegramFilter,
    StringFilter FacebookFilter,
    StringFilter InstagramFilter,
    StringFilter TikTokFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo);