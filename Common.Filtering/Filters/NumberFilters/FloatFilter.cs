using Common.Filtering.FiltersOptions;

namespace Common.Filtering.Filters.NumberFilters;

public record FloatFilter(float Value, NumberFilterOption Option) : BaseFilter<float>(Value);