using Common.Filtering.FiltersOptions;

namespace Common.Filtering.Filters;

public record StringFilter(string Value, StringFilterOption Option) : BaseFilter<string>(Value);
