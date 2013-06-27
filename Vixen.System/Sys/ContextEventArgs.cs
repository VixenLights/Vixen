using System;
using Vixen.Execution;

namespace Vixen.Sys
{
	public class ContextEventArgs : EventArgs
	{
		public ContextEventArgs(IContext context)
		{
			Context = context;
		}

		public IContext Context { get; private set; }
	}
}