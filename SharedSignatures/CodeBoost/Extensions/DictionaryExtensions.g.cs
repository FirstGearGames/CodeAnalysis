using System.Collections.Generic;
using CodeBoost.Performance;
using System;
using System.Threading.Tasks;
namespace CodeBoost.Extensions
{
 public static class DictionaryExtensions
	{
	    /// <summary>
	    /// Returns values as a list.
	    /// </summary>
	    /// <remarks>The returned list is taken from a collection pool.</remarks>
	    public static List<T1> ValuesToList<T0, T1>(this IDictionary<T0, T1> dict)
	    {
	        return default !;
	    }
	
	    /// <summary>
	    /// Clears a list and populates it with the values of a dictionary.
	    /// </summary>
	    public static void ValuesToList<T0, T1>(this IDictionary<T0, T1> dict, ref List<T1> result)
	    {
	    }
	
	    /// <summary>
	    /// Returns keys as a list.
	    /// </summary>
	    /// <remarks>The returned list is taken from a collection pool.</remarks>
	    public static List<T1> KeysToList<T0, T1>(this IDictionary<T0, T1> dict)
	    {
	        return default !;
	    }
	
	    /// <summary>
	    /// Clears a list and populates it with the keys of a dictionary.
	    /// </summary>
	    public static void KeysToList<T0, T1>(this IDictionary<T0, T1> dict, ref List<T0> result)
	    {
	    }
	}
}
