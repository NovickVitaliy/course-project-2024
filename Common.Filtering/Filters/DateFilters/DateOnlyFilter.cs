using Common.Filtering.FiltersOptions;

namespace Common.Filtering.Filters.DateFilters;

public record DateOnlyFilter(DateOnly Value, DateFilterOption Option) : BaseFilter<DateOnly>(Value);