using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.EffectEditor.ChaseEffectEditor
{
	class ChaseEffectEditorDescriptor : EffectEditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{0848b266-0e3a-4090-a283-805792d3bbc7}");
		private static Guid _ChaseId = new Guid("{affea852-85b1-418f-9cdf-0b9735154bb5}");

		public override string Author { get { return "Vixen Team"; } }

		public override string Description { get { return "An editor for the Chase effect."; } }

		public override Type ModuleClass { get { return typeof(ChaseEffectEditor); } }

		public override Guid TypeId { get { return _typeId; } }

		public override string TypeName { get { return "Chase Effect Editor"; } }

		public override string Version { get { return "1.0"; } }

		public override Type[] ParameterSignature { get { return null; } }

		public override Guid EffectTypeId { get { return _ChaseId; } }
	}
}
