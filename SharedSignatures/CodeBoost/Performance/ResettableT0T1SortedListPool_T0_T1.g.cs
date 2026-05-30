using System.Collections.Generic;
using System;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for a SortedList which is resettable.
	/// </summary>
	public static class ResettableT0T1SortedListPool<T0, T1>
	    where T0 : IPoolResettable where T1 : IPoolResettable, new()
	{
	    /// <summary>
	    /// Retrieves an instance of SortedList.
	    /// </summary>
	    public static SortedList<T0, T1> Rent() => default;
	    /// <summary>
	    /// Resets the SortedList, returns it to the pool, and nullifies the reference.
	    /// </summary>
	    public static void ReturnAndNullifyReference(ref SortedList<T0, T1> value)
	    {
	    }

	    /// <summary>
	    /// Resets the SortedList and returns it to the pool.
	    /// </summary>
	    public static void Return(SortedList<T0, T1> value)
	    {
	    }

	    /// <summary>
	    /// Resets the SortedList without returning it to the pool.
	    /// </summary>
	    public static void Reset(SortedList<T0, T1> value)
	    {
	    }
	}
}
