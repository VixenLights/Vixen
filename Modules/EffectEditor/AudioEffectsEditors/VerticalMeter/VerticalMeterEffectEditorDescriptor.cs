using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    internal class VerticalMeterEffectEditorDescriptor : EffectEditorModuleDescriptorBase
    {
        private static Guid _typeId = new Guid("{a6fc24e5-a312-45ab-b777-2e5188520ba8}");
        private static Guid _VerticalMeterId = new Guid("{af92929a-f35e-4762-ba86-a901a1f4eb53}");

        public override string Author
        {
            get { return "Vixen Team"; }
        }

        public override string Description
        {
            get { return "An editor for the Vertical Meter effect."; }
        }

        public override Type ModuleClass
        {
            get { return typeof(VerticalMeterEffectEditor); }
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
            get { return _VerticalMeterId; }
        }
    }
}