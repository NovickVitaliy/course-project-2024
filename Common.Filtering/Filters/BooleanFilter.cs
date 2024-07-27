using Common.Filtering.FiltersOptions;

namespace Common.Filtering.Filters;

public record BooleanFilter(bool Value, BooleanFilterOption Option) : BaseFilter<bool>(Value);