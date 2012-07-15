using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using VixenModules.App.Curves;

namespace VixenModules.EffectEditor.CurveTypeEditor
{
	class CurveTypeEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{b49b34f9-ca0c-44e9-8041-453dd30a1881}");
		private static Guid _CurvesId = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "A control which will edit a parameter of type Curve."; } }

		public override Guid EffectTypeId { get { return Guid.Empty; } }

		public override Type ModuleClass { get { return typeof(CurveTypeEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Curve Type Editor"; } }

		public override string Version { get { return "1.0"; } }

		public override Guid[] Dependencies { get { return new Guid[] { _CurvesId }; } }

		public override Type[] ParameterSignature
		{
			get { return new[] { typeof(Curve) }; }
		}
	}
}
