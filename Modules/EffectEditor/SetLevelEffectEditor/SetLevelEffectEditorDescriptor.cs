using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.EffectEditor.SetLevelEffectEditor
{
	class SetLevelEffectEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{3146f06d-816f-4f94-b978-3e8bb41bd179}");
		private static Guid _SetLevelEffectId = new Guid("{32cff8e0-5b10-4466-a093-0d232c55aac0}");
		private static Guid _LevelTypeEditorId = new Guid("{c3ad317b-3715-418c-9a79-2e315d235648}");
		private static Guid _ColorTypeEditorId = new Guid("{a7a23dee-e08d-47bc-9dc6-cad285675c7d}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "A control which will edit a Set Level effect."; } }

		public override Guid EffectTypeId { get { return _SetLevelEffectId; } }

		public override Type ModuleClass { get { return typeof(SetLevelEffectEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Set Level Effect Editor"; } }

		public override string Version { get { return "0.1"; } }

		public override CommandParameterSignature ParameterSignature { get { return null; } }

		public override Guid[] Dependencies { get { return new Guid[] { _SetLevelEffectId, _LevelTypeEditorId, _ColorTypeEditorId }; } }
	}
}
