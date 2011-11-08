using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.EffectEditor.SpinEffectEditor
{
	class SpinEffectEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{8d5fa0c3-f141-4513-ada1-a3fa61f586e8}");
		private static Guid _SpinId = new Guid("{821a8540-ea34-401f-a8aa-416d7d9a196a}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "An editor for the Spin effect."; } }

		public override Type ModuleClass { get { return typeof(SpinEffectEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Spin Effect Editor"; } }

		public override string Version { get { return "0.1"; } }

		public override Type[] ParameterSignature { get { return null; } }

		public override Guid EffectTypeId { get { return _SpinId; } }
	}
}
