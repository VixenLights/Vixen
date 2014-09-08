using System;
using Vixen.Module.Controller;

namespace VixenModules.Output.GenericSerial
{
	public class Descriptor : ControllerModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{d22c9233-16e9-4008-a215-e577f78a7a2e}");

		public override string Author
		{
			get { return "John McAdams (macebobo)"; }
		}

		public override string Description
		{
			get { return "Generic Serial hardware module"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Module); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (Data); }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string TypeName
		{
			get { return "Generic Serial"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}