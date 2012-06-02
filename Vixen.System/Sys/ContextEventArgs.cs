using System;
using Vixen.Execution;

namespace Vixen.Sys {
	public class ContextEventArgs : EventArgs {
		public ContextEventArgs(Context context) {
			Context = context;
		}

		public Context Context { get; private set; }
	}
}
