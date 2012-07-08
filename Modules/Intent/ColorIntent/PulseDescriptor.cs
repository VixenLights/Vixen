using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Intent;

namespace VixenModules.Intent.Color
{
	public class PulseDescriptor : IntentModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{C66DEB83-1252-4d28-8C50-535ECCE183EC}");
		internal static Guid _RGBPropertyId = new Guid("{5c31be79-a6a7-4864-a660-4e0215ad4778}");
		private static Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");

		public override Guid TypeId { get { return _typeId; } }

		public override Type ModuleClass { get { return typeof(Pulse); } }

		public override Type ModuleDataClass { get { return typeof(PulseData); } }

		public override string Author { get { return "Vixen Team"; } }

		public override string TypeName { get { return "Color intent"; } }

		public override string Description { get { return "Applies a pulse with a variable level and/or color to the target channels."; } }

		public override string Version { get { return "1.0"; } }

		public override Guid[] Dependencies { get { return new Guid[] { _ColorGradientId }; } }
	}
}
