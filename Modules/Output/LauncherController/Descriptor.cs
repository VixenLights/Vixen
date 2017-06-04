using System;
using Vixen.Module.Controller;

namespace VixenModules.Output.LauncherController
{
	public class Descriptor : ControllerModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{1C4BFD64-3172-43A7-A7FC-0A77C88B0489}");

		public override string Author
		{
			get { return "Jon Chuchla / Darren McDaniel / Steve Dupuis"; }
		}

		public override string Description
		{
			get { return "Generic Windows Command / App Launcher"; }
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
			get { return "Launcher"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}