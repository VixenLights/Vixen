using System;
using Vixen.Execution.Context;

namespace Vixen.Sys
{
	public class ProgramContextEventArgs : EventArgs
	{
		public ProgramContextEventArgs(IProgramContext programContext)
		{
			ProgramContext = programContext;
		}

		public IProgramContext ProgramContext { get; private set; }
	}
}