using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    class WaveformEffectEditorDescriptor : EffectEditorModuleDescriptorBase
    {
        private static Guid _typeId = new Guid("{1d4dddcd-a4b1-4244-ac18-7268f2043492}");
        private static Guid _WaveformId = new Guid("{2bedbc3f-b908-4b62-bf4d-6c77ce34be56}");

        public override string Author
        {
            get { return "Vixen Team"; }
        }

        public override string Description
        {
            get { return "An editor for the Waveform effect."; }
        }

        public override Type ModuleClass
        {
            get { return typeof(WaveformEffectEditor); }
        }

        public override Guid TypeId
        {
            get { return _typeId; }
        }

        public override string TypeName
        {
            get { return "Waveform Effect Editor"; }
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override Type[] ParameterSignature
        {
            get { return null; }
        }

        public override Guid EffectTypeId
        {
            get { return _WaveformId; }
        }
    }
}
