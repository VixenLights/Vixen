using System;
using System.Collections.Generic;
using Vixen.Module.Controller;

namespace VixenModules.Output.K8055_Controller
{
	public class K8055Descriptor : ControllerModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{DA33FCFE-827D-4A62-A2B5-14F9F6EBE270}");

		public override string Author
		{
			get { return "Tony Eberle"; }
		}

		public override string Description
		{
			get { return "Velleman K8055 USB Interface board"; }
		}

		public override Type ModuleClass
		{
			get { return typeof(K8055Module); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(K8055Data); }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string TypeName
		{
			get { return "Velleman K8055"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
