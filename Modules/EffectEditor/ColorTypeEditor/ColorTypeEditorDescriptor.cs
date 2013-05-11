using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.ColorTypeEditor
{
	class ColorTypeEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{a7a23dee-e08d-47bc-9dc6-cad285675c7d}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "A control which will edit a parameter of type Color."; } }

		public override Guid EffectTypeId { get { return Guid.Empty; } }

		public override Type ModuleClass { get { return typeof(ColorTypeEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Color Type Editor"; } }

		public override string Version { get { return "1.0"; } }

		public override Type[] ParameterSignature
		{
			get { return new[] { typeof(Color) }; }
		}
	}
}
