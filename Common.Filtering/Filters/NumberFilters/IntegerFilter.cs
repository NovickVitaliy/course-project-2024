using Common.Filtering.FiltersOptions;

namespace Common.Filtering.Filters.NumberFilters;

public record IntegerFilter(int Value, NumberFilterOption Option) : BaseFilter<int>(Value);