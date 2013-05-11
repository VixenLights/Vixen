using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.FilePathTypeEditor {
	public class Module : EffectEditorModuleInstanceBase {
		public override IEffectEditorControl CreateEditorControl() {
			return new FilePathEditorControl();
		}
	}
}
