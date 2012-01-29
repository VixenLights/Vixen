using System;
using Vixen.Module.Intent;

namespace VixenModules.Intent.Pulse
{
	public class PulseDescriptor : IntentModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{55A66507-0EAB-4d3a-97A6-CD467DC32BDA}");
		private static Guid _CurvesId = new Guid("{4e258de2-7a75-4f0f-aa43-c8182e7f3400}");

		public override Guid TypeId { get { return _typeId; } }

		public override Type ModuleClass { get { return typeof(Pulse); } }

		public override Type ModuleDataClass { get { return typeof(PulseData); } }

		public override string Author { get { return "Vixen Team"; } }

		public override string TypeName { get { return "Pulse intent"; } }

		public override string Description { get { return "Applies a pulse with a variable level and/or color to the target channels."; } }

		public override string Version { get { return "1.0"; } }

		public override Guid[] Dependencies { get { return new Guid[] { _CurvesId }; } }

	}
}
