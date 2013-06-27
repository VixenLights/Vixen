using System;
using Vixen.Module.App;

namespace VixenModules.App.SimpleSchedule
{
	public class SimpleScheduleDescriptor : AppModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{8E05A948-0168-40E8-9DF4-7DA57CCCB769}");

		public override string TypeName
		{
			get { return "Simple Schedule"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Scheduler that provides a simplified set of functionality."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (SimpleScheduleModule); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (SimpleSchedulerData); }
		}
	}
}