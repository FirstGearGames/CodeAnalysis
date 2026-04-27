namespace CodeAnalysis.Finding;

/// <summary>
/// Identifies whether a name should include the namespace and containing type.
/// </summary>
public enum NameType
{
    /// <summary>
    /// Name including namespace and containing type.
    /// </summary>
    FullName,
    /// <summary>
    /// Name excluding namespace and containing type.
    /// </summary>
    ShortName,
}

/// <summary>
/// Extension methods for inspecting <see cref="NameType"/> values.
/// </summary>
public static class NameTypeExtensions
{
    /// <summary>
    /// Returns whether the supplied value represents a full name.
    /// </summary>
    /// <param name="thisValue">Value to inspect.</param>
    /// <returns>True when the value is <see cref="NameType.FullName"/>.</returns>
    public static bool IsFullName(this NameType thisValue) => thisValue == NameType.FullName;
    /// <summary>
    /// Returns whether the supplied value represents a short name.
    /// </summary>
    /// <param name="thisValue">Value to inspect.</param>
    /// <returns>True when the value is <see cref="NameType.ShortName"/>.</returns>
    public static bool IsShortName(this NameType thisValue) => thisValue == NameType.ShortName;
}