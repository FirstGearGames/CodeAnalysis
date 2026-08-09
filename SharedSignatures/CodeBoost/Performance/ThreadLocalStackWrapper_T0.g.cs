using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CodeBoost.Performance
{
 public class ThreadLocalStackWrapper<T0>
	{
	    /// <summary>
	    /// Stack for the ThreadLocal.
	    /// </summary>
	    public readonly Stack<T0> LocalStack = [];
	    public ThreadLocalStackWrapper(Action<Stack<T0>> onFinalize)
	    {
	    }
	}
}
