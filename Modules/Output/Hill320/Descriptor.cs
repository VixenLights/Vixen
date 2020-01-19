using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Module;
using Vixen.Module.Controller;

namespace VixenModules.Output.Hill320
{
	internal class Descriptor : ControllerModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{BA739C66-185F-4D0B-8937-67F2C64D1D70}");

		public override string TypeName
		{
			get { return "Hill 320"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Module); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (Data); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Hill 320 hardware module"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}
	}
}