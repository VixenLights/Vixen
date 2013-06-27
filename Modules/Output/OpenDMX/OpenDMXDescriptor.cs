using System;
using Vixen.Module.Controller;

namespace VixenModules.Controller.OpenDMX
{
	internal class VixenOpenDMXDescriptor : ControllerModuleDescriptorBase
	{
		public override string Author
		{
			get { return "Joshua Moyerman (Chilloutdocdoc on DIYC) & piense"; }
		}

		public override string Description
		{
			get { return "Enttec Open DMX Output Module"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (VixenOpenDMXInstance); }
		}

		public override Guid TypeId
		{
			get { return Guid.Parse("7911568a-8eda-4d1d-9e72-be41d7a843e4"); }
		}

		public override int UpdateInterval
		{
			get { return 27; }
		}

		public override string TypeName
		{
			get { return "Open DMX Output"; }
		}

		public override string Version
		{
			get { return "1.5"; }
		}
	}
}