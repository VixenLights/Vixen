using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.LipSyncEditor
{
    public class LipSyncEditorModule : EffectEditorModuleInstanceBase
    {
        public override IEffectEditorControl CreateEditorControl()
        {
            return new LipSyncEditorControl();
        }
    }
}