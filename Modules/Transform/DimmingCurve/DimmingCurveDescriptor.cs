using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Transform;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.Transform.DimmingCurve
{
	class DimmingCurveDescriptor :TransformModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{0cda80e8-9460-46bc-b783-b5d6fd4ca35f}");
		private static Guid _CurvesId = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");

		public override Guid TypeId { get { return _typeId; } }

		public override Type ModuleClass { get { return typeof(DimmingCurve); } }

		public override Type ModuleDataClass { get { return typeof(DimmingCurveData); } }

		public override string Author { get { return "Vixen Team"; } }

		public override string TypeName { get { return "Dimming Curve"; } }

		public override string Description { get { return "Applies a custom curve transform to a monochromatic lighting output that will translate an input level to a different output level."; } }

		public override string Version { get { return "0.1"; } }

		public override Guid[] Dependencies { get { return new Guid[] { _CurvesId }; } }

		public override Type[] TypesAffected
		{
			get { return new Type[] { typeof(Level) }; }
		}

		public override void Setup(ITransform[] transforms)
		{
			// TODO
			throw new NotImplementedException();
		}
	}
}
