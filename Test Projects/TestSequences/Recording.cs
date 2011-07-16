using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;
using Vixen.Sys;
using Vixen.Module.RuntimeBehavior;

namespace TestSequences {
	public class Recording : SequenceModuleInstanceBase {
		public Recording() {
			// Required to execute (length > 0).
			Length = Forever;
			// Enable the live behavior in a bad hacky fashion.
			RuntimeBehaviors.First(x => x.Descriptor.TypeName.Contains("Recording")).Enabled = true;
		}
	}
}
