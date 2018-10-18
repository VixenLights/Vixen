using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VixenModules.Effect.Effect
{
	public class ThreadSafeRandom
	{
		private static readonly Random SeedRandom = new Random();
		private static readonly object LockObject = new object();

		/// <summary> 
		/// Random number generator 
		/// </summary> 
		private static readonly ThreadLocal<Random> ThreadRandom = new ThreadLocal<Random>(NewRandom);

		/// <summary> 
		/// Creates a new instance of Random. The seed is derived 
		/// from a global (static) instance of Random, rather 
		/// than time. 
		/// </summary> 
		private static Random NewRandom()
		{
			lock (LockObject)
			{
				return new Random(SeedRandom.Next());
			}
		}

		/// <summary> 
		/// Returns an instance of Random which can be used freely 
		/// within the current thread. 
		/// </summary> 
		public static Random Instance => ThreadRandom.Value;
	}

	
}
