using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class ProgramEventArgs : EventArgs {
		public ProgramEventArgs(Program program) {
			Program = program;
		}

		public Program Program { get; private set; }
	}
}
