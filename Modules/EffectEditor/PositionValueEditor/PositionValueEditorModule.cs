using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.PositionValueEditor {
	public class PositionValueEditorModule : EffectEditorModuleInstanceBase {
		public override IEffectEditorControl CreateEditorControl() {
			return new PositionValueEditorControl();
		}
	}
}
