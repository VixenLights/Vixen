using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.EffectEditor.PulseEffectEditor
{
	class PulseEffectEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{64ad53bf-c0f3-4828-b97b-4d3e801046c8}");
		private static Guid _PulseEffectId = new Guid("{cbd76d3b-c924-40ff-bad6-d1437b3dbdc0}");
		private static Guid _CurveTypeEditorId = new Guid("{b49b34f9-ca0c-44e9-8041-453dd30a1881}");
		private static Guid _ColorGradientTypeEditorId = new Guid("{eb7397db-bfac-4187-add4-f75b5e8fe773}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "A control which will edit a Pulse effect."; } }

		public override Guid EffectTypeId { get { return _PulseEffectId; } }

		public override Type ModuleClass { get { return typeof(PulseEffectEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Set Level Effect Editor"; } }

		public override string Version { get { return "0.1"; } }

		public override CommandParameterSignature ParameterSignature { get { return null; } }

		public override Guid[] Dependencies { get { return new Guid[] { _PulseEffectId, _CurveTypeEditorId, _ColorGradientTypeEditorId }; } }
	}
}
