using System;
using Vixen.Module.Controller;

namespace VixenModules.Output.CommandController
{
	public class Descriptor : ControllerModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{692B66B9-E831-4F39-BB73-7653FA62B053}");

		public override string Author
		{
			get { return "Darren McDaniel / Steve Dupuis"; }
		}

		public override string Description
		{
			get { return "Generic Windows Command and RDS hardware module"; }
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
			get { return "RDS"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}