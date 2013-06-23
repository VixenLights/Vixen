using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using Vixen.Sys.Attribute;

namespace VixenModules.EffectEditor.NutcrackerEffectEditor
{
    class NutcrackerTypeEditorDescriptor : EffectEditorModuleDescriptorBase
	{
        static NutcrackerTypeEditorDescriptor()
        {
            ModulePath = "Nutcracker";            
        }

        private static Guid _typeId = new Guid("{4577B52A-EF00-4766-A49B-D94A59805742}");

        public override string Author { get { return "Sean Meighan/Derek Backus"; } }

		public override string Description { get { return "A control which will edit a Nutcracker Effect."; } }

		public override Guid EffectTypeId { get { return Guid.Empty; } }

		public override Type ModuleClass { get { return typeof(NutcrackerTypeEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Nutcracker Type Editor"; } }

        [ModuleDataPath]
        public static string ModulePath { get; set; }

		public override string Version { get { return "1.0"; } }

		public override Type[] ParameterSignature
		{
			get
			{
                return new[] { typeof(VixenModules.Effect.Nutcracker.NutcrackerData) };
            }
		}
	}
}
