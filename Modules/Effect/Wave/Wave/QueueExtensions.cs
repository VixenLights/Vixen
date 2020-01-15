using System.Collections.Generic;

namespace VixenModules.Effect.Wave
{
	/// <summary>
	/// Extends the framework Queue class to add the ability to 
	/// pop off the last item added to the queue.
	/// </summary>
	public static class QueueExtension
	{
		/// <summary>
		/// Removes the last item added to the queue.
		/// </summary>		
		public static T Pop<T>(this Queue<T> q)
		{
			for (int i = 1; i < q.Count; i++)
			{
				q.Enqueue(q.Dequeue());
			}

			return q.Dequeue();
		}
	}	
}
