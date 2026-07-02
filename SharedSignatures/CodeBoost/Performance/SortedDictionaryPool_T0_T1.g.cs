using System.Collections.Generic;
using System.Threading;
using CodeBoost.Extensions;
using CodeBoost.Logging;
using System;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for SortedDictionary collections.
	/// </summary>
	public static class SortedDictionaryPool<T0, T1>
	{
	    /// <summary>
	    /// Rents a SortedDictionary.
	    /// </summary>
	    /// <returns>A cleared SortedDictionary collection.</returns>
	    public static SortedDictionary<T0, T1> Rent()
	    {
	        return default !;
	    }

	    /// <summary>
	    /// Returns a SortedDictionary and sets the provided reference to null;
	    /// This Method will not execute if the value is null.
	    /// </summary>
	    /// <param name = "value"> Value to return. </param>
	    public static void ReturnAndNullifyReference(ref SortedDictionary<T0, T1> value)
	    {
	    }

	    /// <summary>
	    /// Returns a SortedDictionary.
	    /// </summary>
	    /// <param name = "value"> Value to return. </param>
	    public static void Return(SortedDictionary<T0, T1> value)
	    {
	    }
	}
}
