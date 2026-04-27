namespace CodeAnalysis.Constants;

/// <summary>
/// Provides full type name constants for commonly referenced framework types.
/// </summary>
public static class NativeConstants
{
    /// <summary>
    /// The full name of the <see cref="System.Func{TResult}"/> open generic type.
    /// </summary>
    public const string FuncFullName = "System.Func";
    /// <summary>
    /// The full name of the <see cref="System.Action"/> type.
    /// </summary>
    public const string ActionFullName = "System.Action";
    /// <summary>
    /// The full name of the <see cref="bool"/> type.
    /// </summary>
    public const string BooleanFullName = "System.Boolean";
    /// <summary>
    /// The full name of the <see cref="ulong"/> type.
    /// </summary>
    public const string UInt64FullName = "System.UInt64";
    /// <summary>
    /// The full name of the <see cref="object"/> type.
    /// </summary>
    public const string ObjectFullName = "System.Object";
    /// <summary>
    /// A literal carriage-return and line-feed sequence used as a line terminator.
    /// </summary>
    public const string LineFeed = "\r\n";
    /// <summary>
    /// The name of the first generic type parameter, namely <c>T0</c>.
    /// </summary>
    public const string FirstGenericParameterName = $"{GenericParameterNamePrefix}0";
    /// <summary>
    /// The prefix used when naming generic type parameters.
    /// </summary>
    public const string GenericParameterNamePrefix = "T";
    /// <summary>
    /// The full name of an array of the first generic type parameter.
    /// </summary>
    public const string GenericArrayFullName = $"{FirstGenericParameterName}[]";
    /// <summary>
    /// The full name of the <see cref="System.Collections.Generic.List{T}"/> open generic type.
    /// </summary>
    public const string ListFullName = "System.Collections.Generic.List";
    /// <summary>
    /// The full name of a list whose element type is the first generic type parameter.
    /// </summary>
    public const string GenericListFullName = $"{ListFullName}<{FirstGenericParameterName}>";
}