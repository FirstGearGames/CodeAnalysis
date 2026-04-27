namespace CodeAnalysis.Finding;

/// <summary>
/// Provides constants used when joining namespace and type names during code analysis.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The character used when joining namespaces for source-level names.
    /// </summary>
    public const string NamespaceNameJoiningCharacter = ".";
    /// <summary>
    /// The character used when joining namespaces for metadata names.
    /// </summary>
    public const string NamespaceMetadataNameJoiningCharacter = "+";

}