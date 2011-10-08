using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Execution;

namespace Vixen.Sys {
	public class ProgramContextEventArgs : EventArgs {
		public ProgramContextEventArgs(ProgramContext programContext) {
			ProgramContext = programContext;
		}

		public ProgramContext ProgramContext { get; private set; }
	}
}
