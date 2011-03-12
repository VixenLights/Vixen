using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Effect {
	[ModuleType("Effect")]
	class EffectModuleImplementation : ModuleImplementation<IEffectModuleInstance> {
		public EffectModuleImplementation()
			: base(new EffectModuleType(), new EffectModuleManagement(), new EffectModuleRepository()) {
		}
	}
}
