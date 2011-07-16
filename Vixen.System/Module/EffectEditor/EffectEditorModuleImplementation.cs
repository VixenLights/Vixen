using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.EffectEditor {
	[TypeOfModule("EffectEditor")]
	class EffectEditorModuleImplementation : ModuleImplementation<IEffectEditorModuleInstance> {
		public EffectEditorModuleImplementation()
			: base(new EffectEditorModuleManagement(), new EffectEditorModuleRepository()) {
		}
	}
}
