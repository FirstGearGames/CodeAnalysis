using System.Collections.Generic;
using System;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for a Queue which is resettable.
	/// </summary>
	public static class ResettableQueuePool<T0>
	    where T0 : IPoolResettable, new()
	{
	    /// <summary>
	    /// Retrieves an instance of Queue.
	    /// </summary>
	    public static Queue<T0> Rent() => default;
	    /// <summary>
	    /// Resets the Queue, returns it to the pool, and nullifies the reference.
	    /// </summary>
	    public static void ReturnAndNullifyReference(ref Queue<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the Queue and returns it to the pool.
	    /// </summary>
	    public static void Return(Queue<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the Queue without returning it to the pool.
	    /// </summary>
	    public static void Reset(Queue<T0> value)
	    {
	    }
	}
}
