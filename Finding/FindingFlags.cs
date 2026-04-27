namespace CodeAnalysis.Finding;

/// <summary>
/// Flags controlling how a finding traversal is performed.
/// </summary>
[System.Flags]
public enum FindingFlags
{
    /// <summary>
    /// Expected value when no flags are present.
    /// </summary>
    None = 0,
    // /// <summary>
    // /// Require NetworkType attribute regardless of any other condition.
    // /// </summary>
    // RequireNetworkTypeAttribute = 1 << 0,
    /// <summary>
    /// Iterate the type recursively.
    /// </summary>
    Recursive = 1 << 1,
}