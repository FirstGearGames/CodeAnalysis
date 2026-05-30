using System.Collections.Generic;
using System;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for a SortedDictionary which is resettable.
	/// </summary>
	public static class ResettableT1SortedDictionaryPool<T0, T1>
	    where T1 : IPoolResettable, new()
	{
	    /// <summary>
	    /// Retrieves an instance of SortedDictionary.
	    /// </summary>
	    public static SortedDictionary<T0, T1> Rent() => default;
	    /// <summary>
	    /// Resets the SortedDictionary, returns it to the pool, and nullifies the reference.
	    /// </summary>
	    public static void ReturnAndNullifyReference(ref SortedDictionary<T0, T1> value)
	    {
	    }

	    /// <summary>
	    /// Resets the SortedDictionary and returns it to the pool.
	    /// </summary>
	    public static void Return(SortedDictionary<T0, T1> value)
	    {
	    }

	    /// <summary>
	    /// Resets the SortedDictionary without returning it to the pool.
	    /// </summary>
	    public static void Reset(SortedDictionary<T0, T1> value)
	    {
	    }
	}
}
