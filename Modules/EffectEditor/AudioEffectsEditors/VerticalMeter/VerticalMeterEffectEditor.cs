using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using Vixen.Module;

namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    internal class VerticalMeterEffectEditor : EffectEditorModuleInstanceBase
    {
        public override IEffectEditorControl CreateEditorControl()
        {
            return new VerticalMeterEffectEditorControl();
        }
    }
}
