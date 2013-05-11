using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.PercentageTypeEditor {
	public class PercentageTypeEditorModule : EffectEditorModuleInstanceBase {
		public override IEffectEditorControl CreateEditorControl() {
			return new PercentageTypeEditorControl();
		}
	}
}
