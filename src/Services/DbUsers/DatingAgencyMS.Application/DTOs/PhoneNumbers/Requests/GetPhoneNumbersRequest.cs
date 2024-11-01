using Common.Filtering.Filters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.PhoneNumbers.Requests;

public record GetPhoneNumbersRequest(
    IntegerFilter IdFilter,
    StringFilter PhoneNumberFilter,
    IntegerFilter AdditionalContactIdFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo);