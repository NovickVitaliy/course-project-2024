using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;

namespace DatingAgencyMS.Client.Features.CoupleArchive.Models.DTOs.Requests;

public record GetArchivedCoupleRequest(
    IntegerFilter CoupleArchiveIdFilter,
    IntegerFilter FirstClientIdFilter,
    IntegerFilter SecondClientIdFilter,
    DateOnlyFilter CoupleCreatedOnFilter,
    StringFilter AddtionalInfoFilter,
    DateTimeFilter ArchivedOnFilter,
    SortingInfo? SortingInfo,
    PaginationInfo PaginationInfo);