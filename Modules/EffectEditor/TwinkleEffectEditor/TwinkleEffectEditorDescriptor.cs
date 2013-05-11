using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.TwinkleEffectEditor
{
	class TwinkleEffectEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{24dbf8dd-d236-40a4-acc4-5a991c955790}");
		private static Guid _TwinkleId = new Guid("{83bdd6f7-19c7-4598-b8e3-7ce28c44e7db}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "An editor for the Twinkle effect."; } }

		public override Type ModuleClass { get { return typeof(TwinkleEffectEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Spin Effect Editor"; } }

		public override string Version { get { return "1.0"; } }

		public override Type[] ParameterSignature { get { return null; } }

		public override Guid EffectTypeId { get { return _TwinkleId; } }
	}
}
