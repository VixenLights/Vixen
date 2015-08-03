using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    class VUMeterEffectEditorDescriptor : EffectEditorModuleDescriptorBase
    {
        private static Guid _typeId = new Guid("{8f568753-b50a-4677-87e7-805213f63879}");
        private static Guid _VUMeterId = new Guid("{8a360703-bf3f-456a-a798-13cd67287dde}");

        public override string Author
        {
            get { return "Vixen Team"; }
        }

        public override string Description
        {
            get { return "An editor for the VU Meter effect."; }
        }

        public override Type ModuleClass
        {
            get { return typeof(VUMeterEffectEditor); }
        }

        public override Guid TypeId
        {
            get { return _typeId; }
        }

        public override string TypeName
        {
            get { return "Vertical Meter Effect Editor"; }
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
            get { return _VUMeterId; }
        }
    }
}
