using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Application.DTOs.Clients.Requests;

public record GetClientsRequest(
    IntegerFilter IdFilter,
    StringFilter FirstNameFilter,
    StringFilter LastNameFilter,
    StringFilter GenderFilter,
    StringFilter SexFilter,
    StringFilter SexualOrientationFilter,
    StringFilter LocationFilter,
    StringFilter RegistrationNumberFilter,
    DateOnlyFilter RegisteredOnFilter,
    IntegerFilter AgeFilter,
    IntegerFilter HeightFilter,
    IntegerFilter WeightFilter,
    StringFilter ZodiacSignFilter,
    StringFilter DescriptionFilter,
    BooleanFilter HasDeclinedServiceFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo,
    string RequestedBy
);