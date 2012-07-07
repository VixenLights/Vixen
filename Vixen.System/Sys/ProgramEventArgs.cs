using System;

namespace Vixen.Sys {
	public class ProgramEventArgs : EventArgs {
		public ProgramEventArgs(IProgram program) {
			Program = program;
		}

		public IProgram Program { get; private set; }
	}
}
