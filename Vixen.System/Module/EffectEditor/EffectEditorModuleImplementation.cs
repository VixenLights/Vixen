using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.EffectEditor {
	[ModuleType("EffectEditor")]
	class EffectEditorModuleImplementation : ModuleImplementation<IEffectEditorModuleInstance> {
		public EffectEditorModuleImplementation()
			: base(new EffectEditorModuleType(), new EffectEditorModuleManagement(), new EffectEditorModuleRepository()) {
		}
	}
}
