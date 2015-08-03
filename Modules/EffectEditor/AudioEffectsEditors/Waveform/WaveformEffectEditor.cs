using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    internal class WaveformEffectEditor : EffectEditorModuleInstanceBase
    {
        public override IEffectEditorControl CreateEditorControl()
        {
            return new WaveformEffectEditorControl();
        }
    }
}
