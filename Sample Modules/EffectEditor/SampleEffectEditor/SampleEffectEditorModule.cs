using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace SampleEffectEditor {
	public class SampleEffectEditorModule : EffectEditorModuleInstanceBase {
		public override IEffectEditorControl CreateEditorControl() {
			return new SampleEffectEditorControl();
		}
	}
}
