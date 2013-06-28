using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.AlternatingEditor
{
    public class AlternatingEffectEditorDescriptor : EffectEditorModuleDescriptorBase
    {
        private static Guid _typeId = new Guid("{83F81B24-E4EC-4A6F-B3DD-A87A8F6454D3}");
        private static Guid _effectID = new Guid("{7B791008-56A2-4BFF-8CE3-A7FB89EA4637}");

        public override string Author { get { return "Darren McDaniel"; } }

        public override string Description { get { return "An editor for the Alternating Colors effect."; } }

        public override Type ModuleClass { get { return typeof(AlternatingEffect); } }

        public override Guid TypeId { get { return _typeId; } }

        public override string TypeName { get { return "Alternting Effect Editor"; } }

        public override string Version { get { return "1.0"; } }

        public override Type[] ParameterSignature { get { return null; } }

        public override Guid EffectTypeId { get { return _effectID; } }
    }
}
