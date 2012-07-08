using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.LevelTypeEditor
{
	class LevelTypeEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{c3ad317b-3715-418c-9a79-2e315d235648}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "A control which will edit a parameter of type double."; } }

		public override Guid EffectTypeId { get { return Guid.Empty; } }

		public override Type ModuleClass { get { return typeof(LevelTypeEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Level Type Editor"; } }

		public override string Version { get { return "1.0"; } }

		public override Type[] ParameterSignature
		{
			get
			{ return new[] { typeof(double) }; }
		}
	}
}
