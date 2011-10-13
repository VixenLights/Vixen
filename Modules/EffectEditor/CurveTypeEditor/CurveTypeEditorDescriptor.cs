using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using VixenModules.App.Curves;

namespace VixenModules.EffectEditor.CurveTypeEditor
{
	class CurveTypeEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{b49b34f9-ca0c-44e9-8041-453dd30a1881}");
		private CommandParameterSignature _paramSpec = new CommandParameterSignature(
			new CommandParameterSpecification("Curve", typeof(Curve))
			);

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "A control which will edit a parameter of type Curve."; } }

		public override Guid EffectTypeId { get { return Guid.Empty; } }

		public override Type ModuleClass { get { return typeof(CurveTypeEditor); } }

		public override CommandParameterSignature ParameterSignature { get { return _paramSpec; } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Curve Type Editor"; } }

		public override string Version { get { return "0.1"; } }
	}
}
