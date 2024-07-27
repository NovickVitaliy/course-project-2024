using Common.Filtering.FiltersOptions;

namespace Common.Filtering.Filters.NumberFilters;

public record DoubleFilter(double Value, NumberFilterOption Option) : BaseFilter<double>(Value);