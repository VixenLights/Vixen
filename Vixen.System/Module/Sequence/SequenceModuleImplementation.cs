using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Module.Sequence {
	[TypeOfModule("Sequence")]
	class SequenceModuleImplementation : ModuleImplementation<ISequenceModuleInstance> {
		public SequenceModuleImplementation()
			: base(new SequenceModuleManagement(), new SequenceModuleRepository()) {
		}
	}
}
