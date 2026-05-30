using System.Collections.Generic;
using System;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 /// <summary>
	/// A pool for a Dictionary which is resettable.
	/// </summary>
	public static class ResettableT0T1DictionaryPool<T0, T1>
	    where T0 : IPoolResettable where T1 : IPoolResettable, new()
	{
	    /// <summary>
	    /// Retrieves an instance of Dictionary.
	    /// </summary>
	    public static Dictionary<T0, T1> Rent() => default;
	    /// <summary>
	    /// Resets the Dictionary, returns it to the pool, and nullifies the reference.
	    /// </summary>
	    public static void ReturnAndNullifyReference(ref Dictionary<T0, T1> value)
	    {
	    }

	    /// <summary>
	    /// Resets the Dictionary and returns it to the pool.
	    /// </summary>
	    public static void Return(Dictionary<T0, T1> value)
	    {
	    }

	    /// <summary>
	    /// Resets the Dictionary without returning it to the pool.
	    /// </summary>
	    public static void Reset(Dictionary<T0, T1> value)
	    {
	    }
	}
}
