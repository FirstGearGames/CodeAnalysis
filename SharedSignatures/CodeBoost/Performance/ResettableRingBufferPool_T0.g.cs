using CodeBoost.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for a RingBuffer which is resettable.
	/// </summary>
	public static class ResettableRingBufferPool<T0>
	    where T0 : IPoolResettable, new()
	{
	    /// <summary>
	    /// Retrieves an instance of RingBuffer.
	    /// </summary>
	    public static RingBuffer<T0> Rent() => default;
	    /// <summary>
	    /// Resets the RingBuffer, returns it to the pool, and nullifies the reference.
	    /// </summary>
	    /// <param name = "value"> Value to return. </param>
	    public static void ReturnAndNullifyReference(ref RingBuffer<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the RingBuffer and returns it to the pool.
	    /// </summary>
	    /// <param name = "value"> Value to return. </param>
	    public static void Return(RingBuffer<T0> value)
	    {
	    }

	    /// <summary>
	    /// Resets the RingBuffer without returning it to the pool.
	    /// </summary>
	    /// <param name = "value"> Value to reset. </param>
	    public static void Reset(RingBuffer<T0> value)
	    {
	    }
	}
}
