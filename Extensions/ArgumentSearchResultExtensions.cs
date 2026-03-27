using Nucleus.CodeAnalysis.SourceGenerators.Finding;

namespace Nucleus.CodeAnalysis.SourceGenerators.Extensions;


public static class ArgumentSearchResultExtensions
{
    public static bool HasError(this ArgumentSearchResult thisArgumentSearchResult) => thisArgumentSearchResult.HasFlag(ArgumentSearchResult.ErrorForSearchType);
}
