using Common.Filtering.Filters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;

public record GetPartnersRequirementRequest(
    IntegerFilter IdFilter,
    StringFilter GenderFilter,
    StringFilter SexFilter,
    IntegerFilter MinAgeFilter,
    IntegerFilter MaxAgeFilter,
    IntegerFilter MinHeightFilter,
    IntegerFilter MaxHeightFilter,
    IntegerFilter MinWeightFilter,
    IntegerFilter MaxWeightFilter,
    StringFilter ZodiacSignFilter,
    StringFilter LocationFilter,
    IntegerFilter ClientIdFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo,
    string RequestedBy);