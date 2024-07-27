using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;

public record GetClientsRequest(
    IntegerFilter IdFilter,
    StringFilter FirstNameFilter,
    StringFilter LastNameFilter,
    StringFilter GenderFilter,
    StringFilter SexualOrientationFilter,
    StringFilter RegistrationNumberFilter,
    DateOnlyFilter RegisteredOnFilter,
    IntegerFilter AgeFilter,
    IntegerFilter HeightFilter,
    IntegerFilter WeightFilter,
    StringFilter ZodiacSignFilter,
    StringFilter DescriptionFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo,
    string RequestedBy
);