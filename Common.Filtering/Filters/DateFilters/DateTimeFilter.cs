using Common.Filtering.FiltersOptions;

namespace Common.Filtering.Filters.DateFilters;

public record DateTimeFilter(DateTime Value, DateFilterOption Option) : BaseFilter<DateTime>(Value);