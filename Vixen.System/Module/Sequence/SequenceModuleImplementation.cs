using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Sequence {
	[ModuleType("Sequence")]
	class SequenceModuleImplementation : ModuleImplementation<ISequenceModuleInstance> {
		public SequenceModuleImplementation()
			: base(new SequenceModuleType(), new SequenceModuleManagement(), new SequenceModuleRepository()) {
		}
	}
}
