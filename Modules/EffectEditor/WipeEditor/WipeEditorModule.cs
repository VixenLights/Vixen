using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.WipeEditor {
	public class WipeEditorModule : EffectEditorModuleInstanceBase {
		public override IEffectEditorControl CreateEditorControl() {
			return new WipeEditorControl();
		}
	}
}
