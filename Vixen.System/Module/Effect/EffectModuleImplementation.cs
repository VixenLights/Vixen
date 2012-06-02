using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Module.Effect {
	[TypeOfModule("Effect")]
	class EffectModuleImplementation : ModuleImplementation<IEffectModuleInstance> {
		public EffectModuleImplementation()
			: base(new EffectModuleManagement(), new EffectModuleRepository()) {
		}
	}
}
