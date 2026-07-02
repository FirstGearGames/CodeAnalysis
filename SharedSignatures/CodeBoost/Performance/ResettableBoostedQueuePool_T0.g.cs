using CodeBoost.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for a BoostedQueue which is resettable.
	/// </summary>
	public static class ResettableBoostedQueuePool<T0>
	    where T0 : IPoolResettable, new()
	{
	    /// <summary>
	    /// Retrieves an instance of BoostedQueue.
	    /// </summary>
	    public static BoostedQueue<T0> Rent() => default;
	    /// <summary>
	    /// Resets the BoostedQueue, returns it to the pool, and nullifies the reference.
	    /// </summary>
	    public static void ReturnAndNullifyReference(ref BoostedQueue<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the BoostedQueue and returns it to the pool.
	    /// </summary>
	    public static void Return(BoostedQueue<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the BoostedQueue without returning it to the pool.
	    /// </summary>
	    public static void Reset(BoostedQueue<T0> value)
	    {
	    }
	}
}
