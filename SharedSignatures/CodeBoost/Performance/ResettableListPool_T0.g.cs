using System.Collections.Generic;
using System;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for a List which is resettable.
	/// </summary>
	public static class ResettableListPool<T0>
	    where T0 : IPoolResettable, new()
	{
	    /// <summary>
	    /// Retrieves an instance of List.
	    /// </summary>
	    public static List<T0> Rent() => default;
	    /// <summary>
	    /// Resets the List, returns it to the pool, and nullifies the reference.
	    /// </summary>
	    /// <param name = "value"> Value to return. </param>
	    public static void ReturnAndNullifyReference(ref List<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the List and returns it to the pool.
	    /// </summary>
	    /// <param name = "value"> Value to return. </param>
	    public static void Return(List<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the List without returning it to the pool.
	    /// </summary>
	    /// <param name = "value"> Value to reset. </param>
	    public static void Reset(List<T0> value)
	    {
	    }
	}
}
