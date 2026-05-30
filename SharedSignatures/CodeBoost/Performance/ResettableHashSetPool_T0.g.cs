using System.Collections.Generic;
using System;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for a HashSet which is resettable.
	/// </summary>
	public static class ResettableHashSetPool<T0>
	    where T0 : IPoolResettable, new()
	{
	    /// <summary>
	    /// Retrieves an instance of HashSet.
	    /// </summary>
	    public static HashSet<T0> Rent() => default;
	    /// <summary>
	    /// Resets the HashSet, returns it to the pool, and nullifies the reference.
	    /// </summary>
	    public static void ReturnAndNullifyReference(ref HashSet<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the HashSet and returns it to the pool.
	    /// </summary>
	    public static void Return(HashSet<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the HashSet without returning it to the pool.
	    /// </summary>
	    public static void Reset(HashSet<T0> value)
	    {
	    }
	}
}
