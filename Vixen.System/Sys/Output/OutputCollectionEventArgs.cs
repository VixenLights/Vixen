using System;

namespace Vixen.Sys.Output
{
	public class OutputCollectionEventArgs<T> : EventArgs
		where T : Output
	{
		public OutputCollectionEventArgs(T output)
		{
			Output = output;
		}

		public T Output { get; private set; }
	}
}