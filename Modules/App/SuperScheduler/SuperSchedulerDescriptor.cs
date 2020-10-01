using System;
using Vixen.Module.App;

namespace VixenModules.App.SuperScheduler
{
	public class SuperSchedulerDescriptor : AppModuleDescriptorBase
	{
		public static Guid _typeId = new Guid("{985315E8-2B83-4D1A-89D1-58BFEA7835A0}");

		public override string TypeName
		{
			get { return "Super Scheduler"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string Author
		{
			get { return "Derek Backus"; }
		}

		public override string Description
		{
			get { return "Expansive Show Scheduler."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof(SuperSchedulerModule); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(SuperSchedulerData); }
		}
	}
}