using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.App;

namespace VixenModules.App.Scheduler
{
	public class SchedulerDescriptor : AppModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{0F4E0F4A-3E9A-4601-8B2A-4D8ADA2674B7}");

		public override string TypeName
		{
			get { return "Show Scheduler"; }
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
			get { return "Schedules automated execution of sequences and programs"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (SchedulerModule); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (SchedulerData); }
		}
	}
}